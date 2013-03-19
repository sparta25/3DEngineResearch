using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;

using TestFramework;

namespace SharpDXTests
{
    
    class Program : ITestable, INotifier, IDisposable
    {
        private RenderForm _form = new RenderForm();
        private DeviceContext _context;
        private RenderTargetView _renderView;
        private readonly ConvexSettings _sceneDescription = SerializationHelper.LoadFromJson<ConvexSettings>(ConfigurationManager.AppSettings["SettingsFile"]);
        
        // Create Device and SwapChain
        private SharpDX.Direct3D11.Device _device;
        private SwapChain _swapChain;
        private Vector4[] _result;
        private DepthStencilView _depthView;
        private SharpDX.Direct3D11.Buffer _constantBuffer;
        private bool _disposed = false;
        private CompilationResult _vertexShaderByteCode;
        private VertexShader _vertexShader;
        private CompilationResult _pixelShaderByteCode;
        private PixelShader _pixelShader;
        private SharpDX.Direct3D11.Buffer _vertices;
        private InputLayout _layout;
        private Texture2D _backBuffer;
        private Factory _factory;
        private readonly ManualResetEvent _disposingFlag = new ManualResetEvent(false);

        [STAThread]
        static void Main(string[] args)
        {
            using (var program = new Program())
            {
                var testHelper = new TestHelper(program);
                testHelper.SetNotifier(program);
                testHelper.CreateScene();
                testHelper.Render();    
            }
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release all resources
                    _vertexShaderByteCode.Dispose();
                    if (_vertexShader != null) _vertexShader.Dispose();
                    if (_pixelShaderByteCode != null) _pixelShaderByteCode.Dispose();
                    if (_pixelShader != null) _pixelShader.Dispose();
                    if (_vertices != null) _vertices.Dispose();
                    if (_layout != null) _layout.Dispose();
                    if (_renderView != null) _renderView.Dispose();
                    if (_backBuffer != null) _backBuffer.Dispose();
                    if (_context != null)
                    {
                        _context.ClearState();
                        _context.Flush();
                        _context.Dispose();
                    }
                    if (_device != null) _device.Dispose();

                    if (_swapChain != null) _swapChain.Dispose();
                    if (_factory != null) _factory.Dispose();
                }
                
                _disposed = true;
            }
        }

        public void Dispose()
        {
            _disposingFlag.Set();
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this); 
        }

        static Vector4 ToVector4(Point point) { return new Vector4(point.X, point.Y, point.Z, 1f); }

        static Vector4 ToVector4(TestFramework.Color color) { return new Vector4(color.Red, color.Green, color.Blue, 1f); }

        #region ITestable
        
        public void CreateScene()
        {
            // SwapChain description
            // Параметры SwapChain, описание смотри ниже
            var desc = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription =
                    new ModeDescription(_form.ClientSize.Width, _form.ClientSize.Height,
                                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = _form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // создаем SwapChain - набор буферов для отрисовки
            // эти буферы необходимы для того, чтобы синхронизировать монитор и конвеер. 
            // Дело в том, безопасно обновлять изображение на мониторе можно только после того, как 
            // будет выведено предидущие изображение.
            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out _device, out _swapChain);
            _context = _device.ImmediateContext;

            // Ignore all windows events
            _factory = _swapChain.GetParent<Factory>();
            _factory.MakeWindowAssociation(_form.Handle, WindowAssociationFlags.IgnoreAll);

            // New RenderTargetView from the backbuffer
            // получаем один буферов из SwapChain.
            // Это фоновый буфер, куда отрисовывается следующие изображение в то время как на экран выводится текущее.
            _backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);
            _renderView = new RenderTargetView(_device, _backBuffer);

            // Compile Vertex and Pixel shaders
            // Читаем из файла шейдер: небольшую подпрограммку для GPU. Vertex shader это вершинный шейдер - подпрограммка 
            // которая принимает на вход матрицу описывающую положение вершины (точки) в пространстве
            // точка входа функция VS
            // vs_4_0 - профиль шейдера, по сути версия шейдера. Видеокарты поддерживают 
            _vertexShaderByteCode = ShaderBytecode.CompileFromFile("MiniCube.fx", "VS", "vs_4_0");
            _vertexShader = new VertexShader(_device, _vertexShaderByteCode);

            // Тоже самое с писсельным шейдером, только имя точки входа тут PS
            _pixelShaderByteCode = ShaderBytecode.CompileFromFile("MiniCube.fx", "PS", "ps_4_0");
            _pixelShader = new PixelShader(_device, _pixelShaderByteCode);

            // Layout from VertexShader input signature
            // Описываем вход стадии InputAssembler, а имеено вершинный шейдер и те данные (которые возьмутся из буфера вершин (см. ниже) которые пойдут на вход этой стадии)
            _layout = new InputLayout(_device, ShaderSignature.GetInputSignature(_vertexShaderByteCode), new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });

            _result = (from x in Enumerable.Range(0, _sceneDescription.PartHeight)
                       from y in Enumerable.Range(0, _sceneDescription.PartWidth)
                       from plane in _sceneDescription.Planes
                       let rect = _sceneDescription.GetQuadrilateralByPosition(plane, x, y)
                       let colorIndex = x * _sceneDescription.PartWidth + y
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
            _vertices = SharpDX.Direct3D11.Buffer.Create(_device, BindFlags.VertexBuffer, _result);

            // Create Constant Buffer
            // буфер констант. Используется для передачи данных между оперативной памятью и памятью видеокарты
            _constantBuffer = new SharpDX.Direct3D11.Buffer(_device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            // Create Depth Buffer & View
            // Буфер глубины он же Z буфер
            var depthBuffer = new Texture2D(_device, new Texture2DDescription
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = _form.ClientSize.Width,
                Height = _form.ClientSize.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            _depthView = new DepthStencilView(_device, depthBuffer);

            // Prepare All the stages
            // Вот тут устанавливаются параметры конвеера, от начальной фазы до конечной
            _context.InputAssembler.InputLayout = _layout;
            // Веселая функция, определяет какие примитивы будут отрисованы (триугольники ил линии, иль еще чего нибудь)
            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertices, Utilities.SizeOf<Vector4>() * 2, 0));
            _context.VertexShader.SetConstantBuffer(0, _constantBuffer);
            _context.VertexShader.Set(_vertexShader);
            _context.Rasterizer.SetViewports(new Viewport(0, 0, _form.ClientSize.Width, _form.ClientSize.Height, 0.0f, 1.0f));
            _context.PixelShader.Set(_pixelShader);

            // Треугольники должны быть видимы в обеих сторон
            var rastStage = RasterizerStateDescription.Default();
            rastStage.CullMode = CullMode.None;
            var rs = new RasterizerState(_context.Device, rastStage);
            _context.Rasterizer.State = rs;
        }

        public void Render()
        {
            // Prepare matrices
            // Готовим матрицы
            // вида     : это вроде как камера
            // проекции : это описание того, как проектировать на экран
            var view = Matrix.LookAtLH(new Vector3(0.0f, 0.0f, -3f), new Vector3(0.2f, 0.2f, 0.2f), Vector3.UnitY);
            var proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, _form.ClientSize.Width / (float)_form.ClientSize.Height, 0.1f, 100.0f);
            // умножая матрицы мы получим матрицу, которая содержит информацию и о камере и о том, как переносить точку из трехмерного пространства на двухмерное,
            // т.е. на экран пользователя
            var viewProj = Matrix.Multiply(view, proj);

            // Use clock
            var clock = new Stopwatch();
            clock.Start();

            RenderLoop.UseCustomDoEvents = false;
            // Main loop
            RenderLoop.Run(_form, () =>
                {
                    OnStart();
                    var time = clock.ElapsedMilliseconds / 1000.0f;

                    // Clear views
                    // чистим от предидущих выводов
                    _context.OutputMerger.SetTargets(_depthView, _renderView);

                    _context.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
                    _context.ClearRenderTargetView(_renderView, SharpDX.Color.Black);

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
                    _context.UpdateSubresource(ref worldViewProj, _constantBuffer);

                    OnFinish();

                    // Draw the cube
                    // отрисовать куб!
                    _context.Draw(_result.Length, 0);

                    // Present!
                    // континиус!
                    _swapChain.Present(0, PresentFlags.None);
                });
        }

        #endregion

        #region INotifier

        public event EventHandler Finish;

        protected virtual void OnFinish()
        {
            EventHandler handler = Finish;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler Start;
        
        protected virtual void OnStart()
        {
            EventHandler handler = Start;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion
    }
}
