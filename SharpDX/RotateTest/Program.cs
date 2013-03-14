using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;

using TestFramework;
using System.Runtime.Serialization.Json;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var form = new RenderForm();

            // SwapChain description
            // Параметры SwapChain, описание смотри ниже
            var desc = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription =
                    new ModeDescription(form.ClientSize.Width, form.ClientSize.Height,
                                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // Create Device and SwapChain
            SharpDX.Direct3D11.Device device;
            SwapChain swapChain;
            // создаем SwapChain - набор буферов для отрисовки
            // эти буферы необходимы для того, чтобы синхронизировать монитор и конвеер. 
            // Дело в том, безопасно обновлять изображение на мониторе можно только после того, как 
            // будет выведено предидущие изображение.
            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
            var context = device.ImmediateContext;

            // Ignore all windows events

            var factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            // New RenderTargetView from the backbuffer
            // получаем один буферов из SwapChain.
            // Это фоновый буфер, куда отрисовывается следующие изображение в то время как на экран выводится текущее.
            var backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            var renderView = new RenderTargetView(device, backBuffer);

            // Compile Vertex and Pixel shaders
            // Читаем из файла шейдер: небольшую подпрограммку для GPU. Vertex shader это вершинный шейдер - подпрограммка 
            // которая принимает на вход матрицу описывающую положение вершины (точки) в пространстве
            // точка входа функция VS
            // vs_4_0 - профиль шейдера, по сути версия шейдера. Видеокарты поддерживают 
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile("MiniCube.fx", "VS", "vs_4_0");
            var vertexShader = new VertexShader(device, vertexShaderByteCode);

            // Тоже самое с писсельным шейдером, только имя точки входа тут PS
            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("MiniCube.fx", "PS", "ps_4_0");
            var pixelShader = new PixelShader(device, pixelShaderByteCode);

            // Layout from VertexShader input signature
            // Описываем вход стадии InputAssembler, а имеено вершинный шейдер и те данные (которые возьмутся из буфера вершин (см. ниже) которые пойдут на вход этой стадии)
            var layout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });

            var settingsSerializer = new DataContractJsonSerializer(typeof(ConvexSettings));
            var sceneDescription = (ConvexSettings)settingsSerializer.ReadObject(new FileStream("Dump.json", FileMode.Open));

            var result = (from x in Enumerable.Range(0, sceneDescription.PartHeight)
                          from y in Enumerable.Range(0, sceneDescription.PartWidth)
                          from plane in sceneDescription.Planes
                          let rect = sceneDescription.GetQuadrilateralByPosition(plane, x, y)
                          let colorIndex = x * sceneDescription.PartWidth + y
                          let color = plane.Colors[colorIndex]
                          select new[] 
                          { 
                              ToVector4(rect.BottomLeft),  ToVector4(color),
                              ToVector4(rect.TopLeft),     ToVector4(color),
                              ToVector4(rect.TopRight),    ToVector4(color),

                              ToVector4(rect.TopRight),    ToVector4(color),
                              ToVector4(rect.BottomRight), ToVector4(color),
                              ToVector4(rect.BottomLeft),  ToVector4(color),
                          })
                         .SelectMany(_ => _)
                         .ToArray();

            // Instantiate Vertex buiffer from vertex data
            // Буфер с описанием вершин ХАРДКОР
            var vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, result);

            // Create Constant Buffer
            // буфер констант. Используется для передачи данных между оперативной памятью и памятью видеокарты
            var contantBuffer = new SharpDX.Direct3D11.Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            // Create Depth Buffer & View
            // Буфер глубины он же Z буфер
            var depthBuffer = new Texture2D(device, new Texture2DDescription
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = form.ClientSize.Width,
                Height = form.ClientSize.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            var depthView = new DepthStencilView(device, depthBuffer);

            // Prepare All the stages
            // Вот тут устанавливаются параметры конвеера, от начальной фазы до конечной
            context.InputAssembler.InputLayout = layout;
            // Веселая функция, определяет какие примитивы будут отрисованы (триугольники ил линии, иль еще чего нибудь)
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, Utilities.SizeOf<Vector4>() * 2, 0));
            context.VertexShader.SetConstantBuffer(0, contantBuffer);
            context.VertexShader.Set(vertexShader);
            context.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
            context.PixelShader.Set(pixelShader);

            var rastStage = SharpDX.Direct3D11.RasterizerStateDescription.Default();
            rastStage.CullMode = CullMode.None;
            var rs = new RasterizerState(context.Device, rastStage);
            context.Rasterizer.State = rs;

            
            // Prepare matrices
            // Готовим матрицы
            // вида     : это вроде как камера
            // проекции : это описание того, как проектировать на экран
            var view = Matrix.LookAtLH(new Vector3(0.0f, 0.0f, -3f), new Vector3(0.2f, 0.2f, 0.2f), Vector3.UnitY);
            var proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 100.0f);
            // умножая матрицы мы получим матрицу, которая содержит информацию и о камере и о том, как переносить точку из трехмерного пространства на двухмерное,
            // т.е. на экран пользователя
            var viewProj = Matrix.Multiply(view, proj);

            // Use clock
            var clock = new Stopwatch();
            clock.Start();
            var frameCount = 0;
            var previousTime = TimeSpan.Zero;

            // Main loop
            RenderLoop.Run(form, () =>
            {
                var time = clock.ElapsedMilliseconds / 1000.0f;

                // Clear views
                // чистим от предидущих выводов
                context.OutputMerger.SetTargets(depthView, renderView);

                context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
                context.ClearRenderTargetView(renderView, SharpDX.Color.Black);

                // Update WorldViewProj Matrix
                var worldViewProj =
                    Matrix.RotationX(time / 3) *
                    Matrix.RotationY(time / 2) *
                    Matrix.RotationZ(time * .1f) *
                    viewProj;
                // нахера транспонировать?
                // Ответ: для перевода привычной системы отсчета в систему отчета по правилу левой руки,
                // в которой работает DirectX
                worldViewProj.Transpose();

                // обновляем данные для шейдеров, а конкретнее матрицу, на которую надо умножить каждую вершину, чтобы повернуть наш куб
                context.UpdateSubresource(ref worldViewProj, contantBuffer);

                if (clock.Elapsed.Seconds != previousTime.Seconds)
                {
                    form.Text = string.Format("FPS: {0}, Planes: {1} Triangles: {2}", frameCount, sceneDescription.Planes.Count, result.Length / 6);
                    frameCount = 0;
                }

                previousTime = clock.Elapsed;

                frameCount++;

                // Draw the cube
                // отрисовать куб!
                context.Draw(result.Length, 0);

                // Present!
                // континиус!
                swapChain.Present(0, SharpDX.DXGI.PresentFlags.None);
            });

            // Release all resources
            vertexShaderByteCode.Dispose();
            vertexShader.Dispose();
            pixelShaderByteCode.Dispose();
            pixelShader.Dispose();
            vertices.Dispose();
            layout.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            context.ClearState();
            context.Flush();
            device.Dispose();
            context.Dispose();
            swapChain.Dispose();
            factory.Dispose();
        }


        static Vector4 ToVector4(Point point) { return new Vector4(point.X, point.Y, point.Z, 1f); }

        static Vector4 ToVector4(TestFramework.Color color) { return new Vector4(color.Red, color.Green, color.Blue, 1f); }

        static Vector4[] GetAllPlane(string fileName)
        {
            return XDocument.Load(fileName).Descendants("Plane").SelectMany(planeTag => GetPlane(planeTag)).ToArray();
        }

        static Vector4[] GetFirstPlane(string fileName)
        {
            return GetPlane(XDocument.Load(fileName).Descendants("Plane").First());
        }

        private static Vector4[] GetPlane(XElement planeTag)
        {
            var cs = from color in planeTag.Element("Colors").Elements()
                     select new
                     {
                         R = float.Parse(color.Element("Red").Value, CultureInfo.InvariantCulture),
                         G = float.Parse(color.Element("Green").Value, CultureInfo.InvariantCulture),
                         B = float.Parse(color.Element("Blue").Value, CultureInfo.InvariantCulture)
                     };
            var ps = from vertice in planeTag.Element("Vertices").Elements()
                     select new
                     {
                         X = float.Parse(vertice.Element("X").Value, CultureInfo.InvariantCulture),
                         Y = float.Parse(vertice.Element("Y").Value, CultureInfo.InvariantCulture),
                         Z = float.Parse(vertice.Element("Z").Value, CultureInfo.InvariantCulture)
                     };

            return cs.Zip(ps, (c, p) => new[] { new Vector4(p.X, p.Y, p.Z, 1f), new Vector4(c.R, c.G, c.B, 1f) }).SelectMany(e => e).ToArray();
        }
    }
}
