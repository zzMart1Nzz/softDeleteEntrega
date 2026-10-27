using System;
using System.Windows.Forms;
using softDeleteEntrega.Services;

namespace softDeleteEntrega
{
    internal static class Program
    {
        /// <summary>
        /// Aplikazioaren sarrera-puntua eta zerbitzuen konexioa.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var api = new ApiZerbitzua())
            {
                var taldearenZerbitzua = new TaldearenZerbitzua(api);
                var form = new Form1(taldearenZerbitzua, api);
                // TODO (INSERT): form.ErreserbaGehitzekoEkintza hemen esleitu.
                // TALDEKIDEENTZAT.md fitxategiak loturaren adibidea dauka.
                Application.Run(form);
            }
        }
    }
}
