﻿using EntityFrameworkModel;
using SpecificationsTesting.Business;
using SpecificationsTesting.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SpecificationsTesting.Forms
{
    public partial class AtexStickerUserControl : UserControl
    {
        private CustomOrder CustomOrder { get; set; }
        public int SelectedVentilatorID { get; set; }
        public string PrinterName { get; set; }
        private ImageSize SelectedImageSize { get; set; }
        private const int NormalImageWidth = 500;
        private const int NormalImageHeight = 700;
        private const int SmallImageWidth = 580;
        private const int SmallImageHeight = 400;

        private enum ImageSize
        {
            Small = 0,
            Medium = 1,
            Large = 2
        }
        public AtexStickerUserControl()
        {
            InitializeComponent();
            this.CustomOrderVentilatorsDataGrid.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.CustomOrderVentilatorsDataGrid_RowEnter);
            this.LogosListBox.SelectedIndexChanged += new System.EventHandler(this.LogosListBox_SelectedIndexChanged);
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);

            PopulateListBox(LogosListBox, Environment.CurrentDirectory + "\\Resources\\Logos", "*.jpg");
            LogosListBox.SelectedIndex = 0;
            SelectedImageSize = ImageSize.Medium;
            ShowTable(SelectedImageSize);
            InitializeGridColumns();
            InitializeGridData();
        }

        private void InitializeGridColumns()
        {
            CustomOrderVentilatorsDataGrid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ID", DataPropertyName = "ID", ReadOnly = true });
            CustomOrderVentilatorsDataGrid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Name", DataPropertyName = "Name", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            CustomOrderVentilatorsDataGrid.RowHeadersVisible = false;
            CustomOrderVentilatorsDataGrid.AutoGenerateColumns = false;
            CustomOrderVentilatorsDataGrid.AllowUserToResizeRows = false;
            CustomOrderVentilatorsDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            CustomOrderVentilatorsDataGrid.MultiSelect = false;
            CustomOrderVentilatorsDataGrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(CustomOrderVentilatorsDataGrid_RowPrePaint);
        }

        private void CustomOrderVentilatorsDataGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts &= ~DataGridViewPaintParts.Focus;
        }

        private void CustomOrderVentilatorsDataGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (int.TryParse(CustomOrderVentilatorsDataGrid.Rows[e.RowIndex].Cells[0].Value.ToString(), out int ventilatorID))
            {
                SelectedVentilatorID = ventilatorID;
                InitializeGridData(false);
            }
        }

        private void InitializeGridData(bool initVentilatorsGrid = true)
        {
            if (CustomOrder != null && initVentilatorsGrid)
            {
                CustomOrderVentilatorsDataGrid.DataSource = null;
                CustomOrderVentilatorsDataGrid.DataSource = CustomOrder.CustomOrderVentilators.ToList();
                CustomOrderVentilatorsDataGrid.AutoResizeColumns();
            }
        }

        private void ShowTable(ImageSize imageSize)
        {
            int imageWidth, imageHeight;
            switch (imageSize)
            {
                case ImageSize.Small:
                    imageWidth = SmallImageWidth;
                    imageHeight = SmallImageHeight;
                    break;
                case ImageSize.Medium:
                    imageWidth = NormalImageWidth;
                    imageHeight = NormalImageHeight;
                    break;
                default:
                    imageWidth = NormalImageWidth;
                    imageHeight = NormalImageHeight;
                    break;
            }
            var image = GenerateTable(imageWidth, imageHeight);
            if (image != null)
            {
                MotorTypePlateImage.Image = (Image)image;
                MotorTypePlateImage.Width = imageWidth;
                MotorTypePlateImage.Height = imageHeight;
            }
        }

        private Image GenerateTable(int imageWidth, int imageHeight)
        {
            if (LogosListBox.SelectedItem == null)
                return null;

            if (CustomOrder == null || CustomOrder.CustomOrderVentilators.Count == 0)
                return null;

            var ventilator = SelectedVentilatorID == 0 ? CustomOrder.CustomOrderVentilators.First() : CustomOrder.CustomOrderVentilators.FirstOrDefault(x => x.ID == SelectedVentilatorID);

            var rows = 20;
            var colWidth = (imageWidth / 2) - 70;
            var rowHeight = 20;
            var startX = 40;
            var startY = 80;
            var pen = new Pen(Color.Black, 2.0F);
            var font = new Font("Tahoma", 8, FontStyle.Bold);

            var logoFile = (FileInfo)LogosListBox.SelectedItem;
            var logo = Image.FromFile(logoFile.FullName);

            var image = new Bitmap(imageWidth, imageHeight);
            using (Graphics graph = Graphics.FromImage(image))
            {
                graph.FillRectangle(Brushes.White, new Rectangle(new Point(0, 0), image.Size));
                graph.DrawImage(logo, new Rectangle(startX, 0, colWidth * 2, 70));

                for (int row = 0; row < rows + 1; row++)
                {
                    var columns = new List<StickerRowColumn>();
                    switch (row)
                    {
                        case 1:
                            columns.Add(new StickerRowColumn() { LeftText = CustomOrder.Reference });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 1, colWidth * 2, columns);
                            break;
                        case 2:
                            columns.Add(new StickerRowColumn() { LeftText = "Order", MiddleText = CustomOrder.CustomOrderNumber.ToString() });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 1, colWidth * 2, columns);
                            break;
                        case 3:
                            columns.Add(new StickerRowColumn() { LeftText = "Serienummer", MiddleText = CustomOrder.CustomOrderNumber.ToString() });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 1, colWidth * 2, columns);
                            break;
                        case 4:
                            columns.Add(new StickerRowColumn() { LeftText = "Bouwjaar", MiddleText = CustomOrder.CreateDate.GetValueOrDefault().Year.ToString() });
                            //TODO: get selected test
                            columns.Add(new StickerRowColumn() { LeftText = "Weight", MiddleText = ventilator.CustomOrderVentilatorTests.First().Weight.ToString() });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 2, colWidth, columns);
                            break;
                        case 5:
                            columns.Add(new StickerRowColumn() { LeftText = "VENTILATOR" });
                            columns.Add(new StickerRowColumn() { LeftText = "MOTOR", MiddleText = ventilator.CustomOrderMotor.Name});
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 2, colWidth, columns);
                            break;
                        case 6:
                            columns.Add(new StickerRowColumn() { LeftText = "Type", MiddleText = ventilator.Name });
                            columns.Add(new StickerRowColumn() { LeftText = "Frequentie", MiddleText = ventilator.CustomOrderMotor.Frequency.ToString() });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 2, colWidth, columns);
                            break;
                        case 7:
                            columns.Add(new StickerRowColumn() { LeftText = "V", MiddleText = DataHelper.CreateHighLowText(ventilator.HighAirVolume.ToString(), ventilator.LowAirVolume.ToString()), RightText = "m3/h" });
                            columns.Add(new StickerRowColumn() { LeftText = "Uitv.", MiddleText = ventilator.CustomOrderMotor.Type });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 2, colWidth, columns);
                            break;
                        case 8:
                            columns.Add(new StickerRowColumn() { LeftText = "Ptot", MiddleText = DataHelper.CreateHighLowText(ventilator.HighPressureTotal.ToString(), ventilator.LowPressureTotal.ToString()), RightText = "Pa" });
                            columns.Add(new StickerRowColumn() { LeftText = "P", MiddleText = DataHelper.CreateHighLowText(ventilator.CustomOrderMotor.HighPower.ToString(), ventilator.CustomOrderMotor.LowPower.ToString()), RightText = "kW" });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 2, colWidth, columns);
                            break;
                        case 9:
                            columns.Add(new StickerRowColumn() { LeftText = "Pst", MiddleText = DataHelper.CreateHighLowText(ventilator.HighPressureStatic.ToString(), ventilator.LowPressureStatic.ToString()), RightText = "Pa" });
                            columns.Add(new StickerRowColumn() { LeftText = "U", MiddleText = ventilator.CustomOrderMotor.VoltageType?.Description, RightText = "V" });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 2, colWidth, columns);
                            break;
                        case 10:
                            columns.Add(new StickerRowColumn() { LeftText = "Pdyn", MiddleText = DataHelper.CreateHighLowText(ventilator.HighPressureDynamic.ToString(), ventilator.LowPressureDynamic.ToString()), RightText = "Pa" });
                            columns.Add(new StickerRowColumn() { LeftText = "Inom", MiddleText = DataHelper.CreateHighLowText(ventilator.CustomOrderMotor.HighPower.ToString(), ventilator.CustomOrderMotor.LowPower.ToString()), RightText = "A" });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 2, colWidth, columns);
                            break;
                        case 11:
                            columns.Add(new StickerRowColumn() { LeftText = "Nvent", MiddleText = DataHelper.CreateHighLowText(ventilator.HighRPM.ToString(), ventilator.LowRPM.ToString()), RightText = "rpm" });
                            columns.Add(new StickerRowColumn() { LeftText = "Istart", MiddleText = ventilator.CustomOrderMotor.StartupAmperage.ToString(), RightText = "A" });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 2, colWidth, columns);
                            break;
                        case 12:
                            columns.Add(new StickerRowColumn() { LeftText = "Schoephoek", RightText = $"{ventilator.BladeAngle}°" });
                            columns.Add(new StickerRowColumn() { LeftText = "Nmotor", MiddleText = DataHelper.CreateHighLowText(ventilator.CustomOrderMotor.HighRPM.ToString(), ventilator.CustomOrderMotor.LowRPM.ToString()), RightText = "rpm" });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 2, colWidth, columns);
                            break;
                        case 13:
                            columns.Add(new StickerRowColumn() { LeftText = "Geluidsvermogen", RightText = $"{ventilator.SoundLevel} {ventilator.SoundLevelType?.UOM}" });
                            columns.Add(new StickerRowColumn() { LeftText = "nr", MiddleText = ventilator.CustomOrderVentilatorTests.FirstOrDefault()?.MotorNumber });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 2, colWidth, columns);
                            break;
                        case 14:
                            columns.Add(new StickerRowColumn() { LeftText = "T", MiddleText = ventilator.TemperatureClass?.Description, RightText = "°C" });
                            columns.Add(new StickerRowColumn() { LeftText = "ρ", RightText = "1,20 kg/m3" });
                            columns.Add(new StickerRowColumn() { LeftText = $"IP {ventilator.CustomOrderMotor.IP}" });
                            columns.Add(new StickerRowColumn() { LeftText = $"ISO {ventilator.CustomOrderMotor.ISO}" });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 4, colWidth / 2, columns);
                            break;
                        case 15:
                            columns.Add(new StickerRowColumn() { LeftText = "Maximum toerental", RightText = $"{ventilator.HighRPM} rpm" });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 1, colWidth * 2, columns);
                            break;
                        case 16:
                            columns.Add(new StickerRowColumn() { LeftText = "Maximum Inlaattemperatuur", RightText = "40 °C" });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 1, colWidth * 2, columns);
                            break;
                        case 17:
                            columns.Add(new StickerRowColumn() { LeftText = "Temperatureklasse", RightText = ventilator.TemperatureClass?.Description });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 1, colWidth * 2, columns);
                            break;
                        case 18:
                            columns.Add(new StickerRowColumn());
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 1, colWidth * 2, columns);
                            break;
                        case 19:
                            columns.Add(new StickerRowColumn() { LeftText = "ATEX markering", RightText = ventilator.Atex });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 1, colWidth * 2, columns);
                            break;
                        case 20:
                            columns.Add(new StickerRowColumn() { LeftText = "Omgevingstemperatuurbereik", RightText = "-20 - +40 °C" });
                            CreateSingleRow(graph, rowHeight, startX, ref startY, 1, colWidth * 2, columns);
                            break;
                        default:
                            break;
                    }
                }
            }
            return (Image)image;
        }

        private void CreateSingleRow(Graphics graph, int rowHeight, int startX, ref int startY, int columnCount, int columnWidth, List<StickerRowColumn> columns)
        {
            var pen = new Pen(Color.Black, 2.0F);
            var font = new Font("Tahoma", 8, FontStyle.Bold);
            var columnStart = startX;
            for (int i = 0; i < columnCount; i++)
            {
                var row = new Rectangle(columnStart, startY, columnWidth, rowHeight);
                graph.DrawRectangle(pen, row);

                StringFormat stringFormat = new StringFormat();
                if(!string.IsNullOrEmpty(columns[i].LeftText))
                {
                    stringFormat.Alignment = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Far;
                    graph.DrawString(columns[i].LeftText, font, Brushes.Black, row, stringFormat);
                }
                if (!string.IsNullOrEmpty(columns[i].MiddleText))
                {
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Far;
                    graph.DrawString(columns[i].MiddleText, font, Brushes.Black, row, stringFormat);
                }
                if (!string.IsNullOrEmpty(columns[i].RightText))
                {
                    stringFormat.Alignment = StringAlignment.Far;
                    stringFormat.LineAlignment = StringAlignment.Far;
                    graph.DrawString(columns[i].RightText, font, Brushes.Black, row, stringFormat);
                }

                columnStart += columnWidth;
            }
            startY += rowHeight;
        }

        private void PopulateListBox(ListBox lsb, string Folder, string FileType)
        {
            DirectoryInfo dinfo = new DirectoryInfo(Folder);
            FileInfo[] Files = dinfo.GetFiles(FileType);
            lsb.DisplayMember = "Name";
            foreach (FileInfo file in Files)
            {
                lsb.Items.Add(file);
            }
        }

        private void LogosListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowTable(SelectedImageSize);
        }

        private void ArrowsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowTable(SelectedImageSize);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ShowCustomOrder();
        }

        public void SelectCustomOrder(int customOrderNumber)
        {
            txtCustomOrderNumber.Text = customOrderNumber.ToString();
            ShowCustomOrder();
        }

        private void ShowCustomOrder()
        {
            if (string.IsNullOrEmpty(txtCustomOrderNumber.Text))
                return;

            var customOrderNumber = int.Parse(txtCustomOrderNumber.Text);
            CustomOrder = BCustomOrder.ByCustomOrderNumber(customOrderNumber);
            if (CustomOrder == null)
            {
                MessageBox.Show($"No order found for number: {customOrderNumber}");
                return;
            }
            InitializeGridData();
            ShowTable(SelectedImageSize);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            var ventilator = CustomOrder.CustomOrderVentilators.FirstOrDefault(x => x.ID == SelectedVentilatorID);
            if (CustomOrder == null || string.IsNullOrEmpty(ventilator.Atex) || !BCustomOrderVentilatorTest.ValidateForPrinting(ventilator))
                return;

            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = PrinterName;
            pd.PrintPage += PrintPage;
            pd.Print();
        }

        private void PrintPage(object o, PrintPageEventArgs e)
        {
            var image = GenerateTable(NormalImageWidth, NormalImageHeight);
            Point loc = new Point(0, 0);
            e.Graphics.DrawImage(image, loc);
        }
    }
}
