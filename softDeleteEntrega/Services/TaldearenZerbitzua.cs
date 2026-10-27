using System;

namespace softDeleteEntrega.Services
{
    // Hiru partial fitxategiek API bezero bera partekatzen dute.
    public partial class TaldearenZerbitzua : ITaldearenZerbitzua
    {
        protected ApiZerbitzua Api { get; }

        public TaldearenZerbitzua(ApiZerbitzua api)
        {
            Api = api ?? throw new ArgumentNullException(nameof(api));
        }
    }
}
