// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// Так, тут у нас структура. Описывает вход вершинного шейдера, т.е. программки по преобразованию вершин
// содержит вектор из 4х флотов для описания координат (:POSITION это указание на семантику. Семантика это описание того, какие именно данные подаются конвеером (см. схему конвеера где угодно в гугле:) в эту переменну.)
// и еще один вектор для описания цвета.
struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
};

// Содержит описание входа пиксельного шейдера 
// аналогично входу вершинного, разные семантики (POSITION и SV_POSITION)
// обусловлены тем, что пиксельный и вершинный шейдер работают на разных этапах конвеера, и соответственно требуют разные типы данных на вход. Под
// 
struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

// это матрица (4 на 4) описывает преобразование, которые необходимо сделать
// чтобы вычислить расположение вершины в пространстве.
// Для всех вершин новая положение (вектор) равняется старому (вектору) умноженному на эту матрицу
float4x4 worldViewProj;

// Эта функция вершинный шейдер. Принимает она VS_IN (пара: вектор-позиция и вектор-цвет, см. описание выше)
// Функция возвращает новые координаты для вершины
// Эти координаты получаются умножением исходных на матрицу
PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
	
	output.pos = mul(input.pos, worldViewProj);
	output.col = input.col;
	
	return output;
}

// Эта функция просто копирует цвет 
float4 PS( PS_IN input ) : SV_Target
{
	return input.col;
}
