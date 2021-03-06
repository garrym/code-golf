﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Size = System.Drawing.Size;

internal class P
{
    private static void Main(string[] a)
    {
        var b = a[0];
        if (!File.Exists(b))
        {
            var h = new HttpClient();
            var m = new Regex("original\":{\"url\":\"(.*?)\"").Matches(h.GetAsync("http://api.giphy.com/v1/gifs/search?q=" + HttpUtility.UrlEncode(b) + "&api_key=dc6zaTOxFJmzC&limit=5&fmt=json").Result.Content.ReadAsStringAsync().Result);
            b = "t.gif";
            File.WriteAllBytes(b, h.GetByteArrayAsync(m[new Random().Next(m.Count)].Groups[1].Value.Replace(@"\/", "/")).Result);
        }
        new H(b);
    }

    private class H
    {
        private const string W0 = "Buffer", W1 = "Window", W2 = "Width", W3 = "Height", W4 = "Largest", J1 = "user32.dll";
        private readonly D C = new D();
        private readonly string K1 = W0 + W2;
        private readonly string K2 = W0 + W3;
        private readonly string K3 = W4 + W1 + W2;
        private readonly string K4 = W4 + W1 + W3;
        private readonly string K5 = W1 + W2;
        private readonly string K6 = W1 + W3;

        private readonly Lazy<List<Color>> P = new Lazy<List<Color>>(() =>
        {
            var c = Enum.GetNames(typeof(ConsoleColor)).Select(Color.FromName).ToList();
            c.Add(Color.FromArgb(255, 128, 128, 0));
            return c;
        });

        private readonly BindingFlags v = BindingFlags.Static | BindingFlags.Public;
        Rect OP;
        public H(string a)
        {
            var f = G(a);
            var n = new Func<I>(() =>
            {
                var r = new I();
                L(f.Count, i => { r.Add(R(f[i])); });
                return r;
            }).Invoke();
            var s = n.Select(d => M((Bitmap)d)).ToList();
            int w = n[0].Width, h = n[0].Height, y = A(K1), q = A(K2), l = A(K3), k = A(K4);
            A(K1, w < y ? y : w);
            A(K2, h < q ? q : h);
            A(K5, w < l ? w : l);
            A(K6, h < k ? h : k);

            GetWindowRect(Process.GetCurrentProcess().MainWindowHandle, ref OP);

            var save = 1;
            var e = new List<Bitmap>();
            var b = 0;
            while (f.Count > 1)
            {
                K p = null;
                L(s.Count, i =>
                {
                    var m = s[i];
                    if (i > 0)
                        p = s[i - 1];
                    var r = E(m, p, save);
                    if (r != null)
                        e.Add(r);
                });
                if (save == 0)
                    continue;
                var g = new GifBitmapEncoder();
                for (var i = 0; i < 3; i++)
                {
                    foreach (var r in e.Select(o => Imaging.CreateBitmapSourceFromHBitmap(o.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())))
                    {
                        g.Frames.Add(BitmapFrame.Create(r));
                    }
                }
                g.Save(new FileStream("1.gif", FileMode.Create));
                save = 0;
            }
            E(s[0], null, save);
        }

        private int A(string x, object y = null)
        {
            if (y != null)
                typeof(Console).GetProperty(x, v).SetValue(null, y);
            return (int)typeof(Console).GetProperty(x, v).GetValue(null);
        }

        private ConsoleColor F(Color c)
        {
            int a = 128, b = 64;
            var i = (c.R > a | c.G > a | c.B > a) ? 8 : 0;
            i |= (c.R > b) ? 4 : 0;
            i |= (c.G > b) ? 2 : 0;
            i |= (c.B > b) ? 1 : 0;
            return (ConsoleColor)i;
        }

        private Bitmap E(K m, K e, int s)
        {
            var h = m.C.GetLength(0);
            var w = m.C.GetLength(1);
            N(h, w, (y, x) =>
            {
                if (e == null || e.C[y, x] != m.C[y, x])
                {
                    var p = m.C[y, x];
                    A("ForegroundColor", p);
                    Console.SetCursorPosition(x, y);
                    Console.Write("/");
                }
            });
            if (s == 0)
                return null;
            try
            {
                var i = new Bitmap(OP.Right - OP.Left, OP.Bottom - OP.Top);
                Graphics.FromImage(i).CopyFromScreen(OP.Left, OP.Top, A(K5), A(K6), new Size(i.Width, i.Height));
                return i;
            }
            catch
            {
                return null;
            }
        }

        [DllImport(J1, CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport(J1)]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        private I G(string p)
        {
            var i = Image.FromFile(p);
            var f = new I();
            try
            {
                var c = i.GetFrameCount(FrameDimension.Time);
                L(c, x =>
                {
                    i.SelectActiveFrame(FrameDimension.Time, x);
                    f.Add((Image)i.Clone());
                });
            }
            catch
            {
                f.Add(i);
            }
            return f;
        }

        private void N(int z, int y, Action<int, int> a)
        {
            for (var i = 0; i < z; i++)
                for (var j = 0; j < y; j++)
                    a.Invoke(i, j);
        }

        private void L(int z, Action<int> a)
        {
            for (var i = 0; i < z; i++)
                a.Invoke(i);
        }

        private K M(Bitmap b)
        {
            var k = new K(b.Height, b.Width);
            N(b.Height, b.Width, (y, x) => { k.C[y, x] = F(X(b.GetPixel(x, y))); });
            return k;
        }

        private Bitmap R(Image i)
        {
            int w = 120, h = 50;
            var d = i.Width / (double)i.Height;
            var p = (int)(h * d);
            var q = (int)(w / d);
            return (int)(h * d) <= w ? new Bitmap(i, p, h) : new Bitmap(i, w, q);
        }

        private Color X(Color c)
        {
            if (C.ContainsKey(c))
                return C[c];
            var x = Y(c);
            C.Add(c, x);
            return x;
        }

        private Color Y(Color s)
        {
            var m = int.MaxValue;
            var r = Color.Black;
            foreach (var c in P.Value)
            {
                int x = s.R - c.R, y = s.G - c.G, z = s.B - c.B;
                var d = x * x + y * y + z * z;
                if (d >= m)
                    continue;
                m = d;
                r = c;
            }
            return r;
        }

        private struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        private class I : List<Image>
        {}

        private class D : Dictionary<Color, Color>
        {}

        private class K
        {
            public readonly ConsoleColor[,] C;

            public K(int h, int w)
            {
                C = new ConsoleColor[h, w];
            }
        }
    }
}