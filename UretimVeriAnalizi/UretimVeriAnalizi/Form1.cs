using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace UretimVeriAnalizi
{
    public partial class Form1 : Form
    {
        int gunSayisi = 8;
        int makineSayisi = 4;
        Random random = new Random();
        DataTable originalDataTable = new DataTable();

        public Form1()
        {
            InitializeComponent();
            gridViewBaslat();
            VeriTablosuBasla();
            VerileriEkle();
            comboBoxMakinelerreEkle();
        }

        private void gridViewBaslat()
        {
            dataGridView1.Columns.Add("Makine", "Makine");
            dataGridView1.Columns.Add("Tarih", "Tarih");
            dataGridView1.Columns.Add("HedefMiktar", "Hedef Miktar");
            dataGridView1.Columns.Add("UretilenMiktar", "Üretilen Miktar");

            dataGridView1.Columns["Makine"].Width = 100;
            dataGridView1.Columns["Tarih"].Width = 100;
            dataGridView1.Columns["HedefMiktar"].Width = 100;
            dataGridView1.Columns["UretilenMiktar"].Width = 100;
        }

        private void VeriTablosuBasla()
        {
            originalDataTable.Columns.Add("Makine");
            originalDataTable.Columns.Add("Tarih");
            originalDataTable.Columns.Add("HedefMiktar");
            originalDataTable.Columns.Add("UretilenMiktar");
        }

        private void VerileriEkle()
        {
            DateTime BasTar = new DateTime(2024, 6, 1);

            for (int Gun = 0; Gun < gunSayisi; Gun++)
            {
                DateTime SimdikiTar = BasTar.AddDays(Gun);
                for (int Makine = 1; Makine <= makineSayisi; Makine++)
                {
                    string makineAdi = "Makine" + Makine;
                    string tarih = SimdikiTar.ToString("dd/MM/yyyy");
                    int hedefMiktar = random.Next(100, 301);
                    int uretilenMiktar = random.Next(100, 301);

                    dataGridView1.Rows.Add(makineAdi, tarih, hedefMiktar, uretilenMiktar);
                    originalDataTable.Rows.Add(makineAdi, tarih, hedefMiktar, uretilenMiktar);
                }
            }
        }

        private void comboBoxMakinelerreEkle()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Hepsi");
            for (int Makine = 1; Makine <= makineSayisi; Makine++)
            {
                string makineAdi = "Makine" + Makine;
                comboBox1.Items.Add(makineAdi);
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FilterDataGridView();
            UpdateChart();
        }

        private void FilterDataGridView()
        {
            string SeciliMakine = comboBox1.SelectedItem.ToString();
            DateTime BasTar = dateTimePickerBas.Value;
            DateTime BitTar = dateTimePickerBitis.Value;

            var filtre = originalDataTable.AsEnumerable().Where(row =>
            {
                DateTime Satir = DateTime.ParseExact(row.Field<string>("Tarih"), "dd/MM/yyyy", null);
                bool TarihlerAra = Satir >= BasTar && Satir <= BitTar;
                bool EslesenMakine = SeciliMakine == "Hepsi" || row.Field<string>("Makine") == SeciliMakine;

                return TarihlerAra && EslesenMakine;
            });

            DataTable filtrelenmisSatir;
            if (filtre.Any())
            {
                filtrelenmisSatir = filtre.CopyToDataTable();
            }
            else
            {
                filtrelenmisSatir = originalDataTable.Clone(); // Boş ama aynı yapıda tablo
            }

            dataGridView1.Rows.Clear();

            foreach (DataRow VeriSatiri in filtrelenmisSatir.Rows)
            {
                dataGridView1.Rows.Add(VeriSatiri.ItemArray);
            }
        }

        private void UpdateChart()
        {
            chart1.Series.Clear();
            var hedefMiktarSeries = new Series("Hedef Miktar");
            var uretilenMiktarSeries = new Series("Üretilen Miktar");

            hedefMiktarSeries.ChartType = SeriesChartType.Column;
            uretilenMiktarSeries.ChartType = SeriesChartType.Column;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Tarih"].Value != null)
                {
                    DateTime date = DateTime.ParseExact(row.Cells["Tarih"].Value.ToString(), "dd/MM/yyyy", null);
                    int hedefMiktar = int.Parse(row.Cells["HedefMiktar"].Value.ToString());
                    int uretilenMiktar = int.Parse(row.Cells["UretilenMiktar"].Value.ToString());

                    hedefMiktarSeries.Points.AddXY(date, hedefMiktar);
                    uretilenMiktarSeries.Points.AddXY(date, uretilenMiktar);
                }
            }

            chart1.Series.Add(hedefMiktarSeries);
            chart1.Series.Add(uretilenMiktarSeries);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
