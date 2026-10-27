using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using softDeleteEntrega.Models;
using softDeleteEntrega.Services;
using softDeleteEntrega.Ui;

namespace softDeleteEntrega
{
    public partial class Form1 : Form
    {
        private readonly ITaldearenZerbitzua taldearenZerbitzua;
        private readonly ApiZerbitzua api;
        private readonly Panel edukia = new Panel();
        private readonly FlowLayoutPanel nabigazioa = new FlowLayoutPanel();
        private readonly Label saioEtiketa = Itxura.Etiketa("", 10);
        private Erabiltzailea erabiltzailea;
        private bool aurreikuspena;
        private bool lanean;
        private DataGridView taula;
        private Label egoera;
        private TextBox bazkideId;
        private bool pistaDesgaituak;

        // INSERT pantaila prest dagoenean, Program.cs-en esleitu ekintza hau.
        // true: erreserba gorde da eta zerrenda berritu behar da; false: bertan behera.
        public Func<Erabiltzailea, Task<bool>> ErreserbaGehitzekoEkintza { get; set; }

        public Form1() : this(new ApiZerbitzua())
        {
        }

        private Form1(ApiZerbitzua api) : this(new TaldearenZerbitzua(api), api)
        {
        }

        public Form1(ITaldearenZerbitzua taldearenZerbitzua, ApiZerbitzua api)
        {
            this.taldearenZerbitzua = taldearenZerbitzua;
            this.api = api;
            InitializeComponent();
            SortuEgitura();
            ErakutsiLogin();
            FormClosed += (sender, e) => this.api.Dispose();
            FormClosing += (sender, e) => { if (lanean) e.Cancel = true; };
        }

        private void SortuEgitura()
        {
            var egitura = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1 };
            egitura.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
            egitura.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            egitura.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            egitura.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            Controls.Add(egitura);

            var goiburua = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, BackColor = Itxura.Iluna, ColumnCount = 2,
                Padding = new Padding(30, 12, 30, 12), Margin = Padding.Empty
            };
            goiburua.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            goiburua.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            var marka = Itxura.Etiketa("KIROLDEGIA\nPisten eta erreserben kudeaketa", 17, true);
            marka.ForeColor = Color.White;
            goiburua.Controls.Add(marka, 0, 0);
            saioEtiketa.ForeColor = Color.FromArgb(192, 218, 228);
            saioEtiketa.TextAlign = ContentAlignment.MiddleRight;
            goiburua.Controls.Add(saioEtiketa, 1, 0);
            egitura.Controls.Add(goiburua, 0, 0);

            nabigazioa.Dock = DockStyle.Fill;
            nabigazioa.Padding = new Padding(30, 8, 0, 0);
            nabigazioa.Margin = Padding.Empty;
            egitura.Controls.Add(nabigazioa, 0, 1);
            edukia.Dock = DockStyle.Fill;
            edukia.Padding = new Padding(30, 8, 30, 12);
            edukia.Margin = Padding.Empty;
            egitura.Controls.Add(edukia, 0, 2);
            var oina = Itxura.Etiketa("KIROLDEGIA   /   Erreserbak eta pistak", 9);
            oina.Padding = new Padding(30, 0, 0, 0);
            oina.ForeColor = Itxura.Apala;
            egitura.Controls.Add(oina, 0, 3);
        }

        private void GarbituEdukia()
        {
            AcceptButton = null;
            taula = null;
            egoera = null;
            while (edukia.Controls.Count > 0) edukia.Controls[0].Dispose();
            while (nabigazioa.Controls.Count > 0) nabigazioa.Controls[0].Dispose();
        }

        private TableLayoutPanel Orria(string title, string description, bool withTable = false)
        {
            GarbituEdukia();
            var page = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = withTable ? 5 : 3
            };
            page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            if (withTable)
            {
                page.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
                page.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                page.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            }
            else page.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            page.Controls.Add(Itxura.Etiketa(title, 25, true), 0, 0);
            var azpititulua = Itxura.Etiketa(description, 10);
            azpititulua.ForeColor = Itxura.Apala;
            page.Controls.Add(azpititulua, 0, 1);
            edukia.Controls.Add(page);
            return page;
        }

        private void Nabigazioa(Action atzera)
        {
            if (atzera != null)
            {
                var back = Itxura.Botoia("‹  Atzera", Itxura.Bigarrena, 105);
                back.Click += (sender, e) => atzera();
                nabigazioa.Controls.Add(back);
            }
            var home = Itxura.Botoia("Hasiera", Itxura.Iluna, 105);
            home.Click += (sender, e) => ErakutsiHasiera();
            nabigazioa.Controls.Add(home);
            var logout = Itxura.Botoia(aurreikuspena ? "Aurreikuspena itxi" : "Saioa itxi", Itxura.Apala, 170);
            logout.Click += (sender, e) => ErakutsiLogin();
            nabigazioa.Controls.Add(logout);
        }

        private void ErakutsiLogin()
        {
            erabiltzailea = null;
            aurreikuspena = false;
            saioEtiketa.Text = "Ongi etorri";
            var page = Orria("Hasi saioa", "Sartu zure bazkidearen ID zenbakia.");
            var center = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            center.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            center.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430));
            center.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            page.Controls.Add(center, 0, 2);
            var card = new TableLayoutPanel
            {
                Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, RowCount = 5,
                Padding = new Padding(26), BackColor = Color.White, Margin = new Padding(0, 8, 0, 0)
            };
            card.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (int height in new[] { 30, 54, 54, 62, 42 })
                card.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            center.Controls.Add(card, 1, 0);
            card.Controls.Add(Itxura.Etiketa("Bazkidearen ID-a", 10, true), 0, 0);
            bazkideId = new TextBox
            {
                Dock = DockStyle.Top, Font = new Font("Segoe UI", 13),
                MaxLength = 10, AccessibleName = "Bazkidearen ID-a"
            };
            card.Controls.Add(bazkideId, 0, 1);
            var login = Itxura.Botoia("Sartu", Itxura.Nagusia);
            login.Dock = DockStyle.Fill;
            login.Margin = new Padding(0, 3, 0, 9);
            login.Click += async (sender, e) => await HasiSaioaAsync();
            card.Controls.Add(login, 0, 2);
            AcceptButton = login;
            egoera = Itxura.Etiketa("Zure ID-a bazkideen zerrendan egiaztatuko da.", 9);
            egoera.ForeColor = Itxura.Apala;
            card.Controls.Add(egoera, 0, 3);
            var preview = new LinkLabel
            {
                Text = "Interfazea aurreikusi (saioa hasi gabe)", Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter, LinkColor = Itxura.Nagusia
            };
            preview.LinkClicked += (sender, e) =>
            {
                aurreikuspena = true;
                saioEtiketa.Text = "AURREIKUSPENA\nSaioa hasi gabe";
                ErakutsiHasiera();
            };
            card.Controls.Add(preview, 0, 4);
            ActiveControl = bazkideId;
        }

        private async Task HasiSaioaAsync()
        {
            if (lanean) return;
            int id;
            if (!int.TryParse(bazkideId.Text.Trim(), System.Globalization.NumberStyles.None,
                System.Globalization.CultureInfo.InvariantCulture, out id) || id <= 0)
            {
                Mezua("Sartu bazkidearen ID zenbaki oso positibo bat.", true);
                return;
            }
            Erabiltzailea saioa = null;
            await EginAsync(async () =>
            {
                Mezua("Bazkidea egiaztatzen…");
                saioa = await api.LoginAsync(id);
                if (saioa == null || saioa.Id <= 0)
                    Mezua("Ez dago ID hori duen bazkiderik.", true);
            });
            if (saioa == null || saioa.Id <= 0) return;
            erabiltzailea = saioa;
            aurreikuspena = false;
            saioEtiketa.Text = "Bazkidea #" + erabiltzailea.Id + "\n" + erabiltzailea.IzenOsoa;
            ErakutsiHasiera();
        }

        private void ErakutsiHasiera()
        {
            var page = Orria("Zer kudeatu nahi duzu?", "Aukeratu atal bat hasteko.");
            Nabigazioa(null);
            BiBotoi(page, "Erreserbak\n\nZerrenda ikusi eta sarrerak gehitu", async () => await ErakutsiErreserbakAsync(),
                "Pistak\n\nPistak eta desgaitutako pistak", () => ErakutsiPistenMenua());
        }

        private void ErakutsiPistenMenua()
        {
            var page = Orria("Pistak", "Aukeratu ikusi nahi duzun pisten zerrenda.");
            Nabigazioa(ErakutsiHasiera);
            BiBotoi(page, "Pistak\n\nPista erabilgarriak", async () => await ErakutsiPistakAsync(false),
                "Pista desgaituak\n\nBerreskuratu edo behin betiko ezabatu", async () => await ErakutsiPistakAsync(true));
        }

        private static void BiBotoi(TableLayoutPanel page, string firstText, Action firstAction,
            string secondText, Action secondAction)
        {
            var cards = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            cards.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var first = Itxura.Txartela(firstText, Itxura.Nagusia);
            var second = Itxura.Txartela(secondText, Itxura.Bigarrena);
            first.Click += (sender, e) => firstAction();
            second.Click += (sender, e) => secondAction();
            cards.Controls.Add(first, 0, 0);
            cards.Controls.Add(second, 1, 0);
            page.Controls.Add(cards, 0, 2);
        }

        private FlowLayoutPanel PrestatuTaula(TableLayoutPanel page)
        {
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(0, 6, 0, 0) };
            page.Controls.Add(actions, 0, 2);
            taula = Itxura.Taula();
            page.Controls.Add(taula, 0, 3);
            egoera = Itxura.Etiketa("Zerrenda kargatzen…", 10);
            page.Controls.Add(egoera, 0, 4);
            return actions;
        }

        private async Task ErakutsiErreserbakAsync()
        {
            var page = Orria("Erreserbak", "Kontsultatu erreserbak edo gehitu sarrera bat.", true);
            Nabigazioa(ErakutsiHasiera);
            var actions = PrestatuTaula(page);
            var gehitu = Itxura.Botoia("+  Erreserba gehitu", Itxura.Nagusia, 200);
            gehitu.Click += async (sender, e) => await EskatuErreserbaGehitzeaAsync();
            actions.Controls.Add(gehitu);
            var refresh = Itxura.Botoia("Berritu", Itxura.Bigarrena, 110);
            refresh.Click += async (sender, e) => await KargatuErreserbakAsync();
            actions.Controls.Add(refresh);
            Itxura.Zutabea(taula, "Id", "ID", 45);
            Itxura.Zutabea(taula, "Pista_id", "Pista ID");
            Itxura.Zutabea(taula, "Bazkidea_id", "Bazkide ID");
            Itxura.Zutabea(taula, "Data", "Data", 120);
            taula.Columns["Data"].DefaultCellStyle.Format = "yyyy-MM-dd";
            Itxura.Zutabea(taula, "Ordua", "Ordua");
            taula.Columns["Ordua"].DefaultCellStyle.Format = @"hh\:mm";
            await KargatuErreserbakAsync();
        }

        private Task KargatuErreserbakAsync()
        {
            return EginAsync(async () =>
            {
                var rows = await taldearenZerbitzua.LortuErreserbakAsync();
                taula.DataSource = new BindingList<Erreserba>(rows);
                Mezua(rows.Count == 0 ? "Ez dago erreserbarik." : rows.Count + " erreserba.");
            });
        }

        private async Task EskatuErreserbaGehitzeaAsync()
        {
            if (lanean) return;
            if (erabiltzailea == null || aurreikuspena)
            {
                Mezua("Ekintza hau egiteko, hasi saioa.", true);
                return;
            }
            if (ErreserbaGehitzekoEkintza == null)
            {
                Mezua("Erreserbak gehitzeko aukera oraindik ez dago erabilgarri.");
                return;
            }
            bool gehituta = false;
            await EginAsync(async () => gehituta = await ErreserbaGehitzekoEkintza(erabiltzailea));
            if (gehituta) await KargatuErreserbakAsync();
        }

        private async Task ErakutsiPistakAsync(bool desgaituta)
        {
            pistaDesgaituak = desgaituta;
            var page = Orria(desgaituta ? "Pista desgaituak" : "Pistak",
                desgaituta ? "Berreskuratu pista bat edo aukeratu behin betiko ezabatzea."
                    : "Pista bat ezabatzean, pista desgaituen zerrendara igaroko da.", true);
            Nabigazioa(ErakutsiPistenMenua);
            var actions = PrestatuTaula(page);
            var refresh = Itxura.Botoia("Berritu", Itxura.Bigarrena, 110);
            refresh.Click += async (sender, e) => await KargatuPistakAsync();
            actions.Controls.Add(refresh);
            Itxura.Zutabea(taula, "Id", "ID", 40);
            Itxura.Zutabea(taula, "Izena", "Izena", 150);
            Itxura.Zutabea(taula, "Kirola", "Kirola", 100);
            if (desgaituta) Itxura.Ekintza(taula, "Berreskuratu", "Berreskuratu", Itxura.Nagusia);
            Itxura.Ekintza(taula, "Ezabatu", "Ezabatu", Color.FromArgb(165, 58, 62));
            taula.CellContentClick += async (sender, e) => await PistaEkintzaAsync(e);
            await KargatuPistakAsync();
        }

        private Task KargatuPistakAsync()
        {
            return EginAsync(async () =>
            {
                // SELECT/iragazkia taldekidearen zerbitzuari dagokio, ez interfazeari.
                var rows = await taldearenZerbitzua.LortuPistakAsync(pistaDesgaituak);
                taula.DataSource = new BindingList<Pista>(rows);
                Mezua(rows.Count == 0 ? "Ez dago pistarik zerrenda honetan." : rows.Count + " pista.");
            });
        }

        private async Task PistaEkintzaAsync(DataGridViewCellEventArgs e)
        {
            if (lanean || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var action = taula.Columns[e.ColumnIndex].Name;
            if (action != "Ezabatu" && action != "Berreskuratu") return;
            var pista = taula.Rows[e.RowIndex].DataBoundItem as Pista;
            if (pista == null) return;
            if (erabiltzailea == null || aurreikuspena)
            {
                Mezua("Ekintza hau egiteko, hasi saioa.", true);
                return;
            }

            if (pistaDesgaituak && action == "Ezabatu")
            {
                // DELETE funtzioa taldekideak inplementatuko du; ez da soft delete deitzen.
                await EginAsync(async () =>
                {
                    await taldearenZerbitzua.EzabatuPistaAsync(pista.Id);
                    ((BindingList<Pista>)taula.DataSource).Remove(pista);
                    Mezua("Pista behin betiko ezabatu da.");
                });
                return;
            }

            bool berreskuratu = action == "Berreskuratu";
            string galdera = berreskuratu
                ? "\"" + pista.Izena + "\" pista eta hari lotutako erreserba guztiak berreskuratu nahi dituzu?"
                : "\"" + pista.Izena + "\" pista eta hari lotutako erreserba guztiak desgaitu nahi dituzu?";
            if (MessageBox.Show(this, galdera, berreskuratu ? "Pista berreskuratu" : "Pista desgaitu",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            await EginAsync(async () =>
            {
                if (berreskuratu) await api.BerreskuratuAsync(pista.Id);
                else await api.DesgaituAsync(pista.Id);
                // Errenkada APIak arrakasta baieztatu ondoren bakarrik kentzen da.
                ((BindingList<Pista>)taula.DataSource).Remove(pista);
                Mezua(berreskuratu ? "Pista eta haren erreserbak berreskuratu dira."
                    : "Pista eta haren erreserbak desgaitu dira.");
            });
        }

        private void Mezua(string message, bool error = false)
        {
            if (egoera == null) return;
            egoera.Text = message;
            egoera.ForeColor = error ? Color.FromArgb(165, 58, 62) : Itxura.Apala;
        }

        private async Task EginAsync(Func<Task> action)
        {
            if (lanean) return;
            lanean = true;
            edukia.Enabled = false;
            nabigazioa.Enabled = false;
            UseWaitCursor = true;
            try
            {
                await action();
            }
            catch (NotImplementedException exception)
            {
                Mezua(exception.Message);
            }
            catch (Exception exception)
            {
                Mezua(exception.Message, true);
            }
            finally
            {
                lanean = false;
                edukia.Enabled = true;
                nabigazioa.Enabled = true;
                UseWaitCursor = false;
            }
        }
    }
}
