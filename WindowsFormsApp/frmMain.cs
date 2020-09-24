using DEM.Net.Core;
using DEM.Net.Core.Imagery;
using DEM.Net.Extension.VisualTopo;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Research.Science.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class frmMain : Form
    {
        string visualTopoFile = null;
        private readonly DemNetVisualTopoService demNetService;
        private readonly DEMDataSet dataSet = DEMDataSet.AW3D30;

        // Generated file paths
        string _excelFile;
        string _glbFile;

        public frmMain()
        {
            InitializeComponent();

            demNetService = new DemNetVisualTopoService();

            cmbImagery.DataSource = demNetService.GetImageryProviders();
        }

        private void btnOpenVTopoFile_Click(object sender, EventArgs e)
        {
            SetVisualTopoFile(null);

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "VisualTopo (*.tro)|*.tro";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    SetVisualTopoFile(openFileDialog.FileName);

                }
            }
        }

        private void SetVisualTopoFile(string fileName)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                visualTopoFile = File.Exists(fileName) ? fileName : null;

                if (visualTopoFile == null)
                {
                    lblStatus.Text = "Aucun fichier chargé.";
                    btnExport3D.Enabled = false;
                    btnExport.Enabled = false;
                }
                else
                {
                    lblStatus.Text = $"{Path.GetFileName(fileName)}";
                    btnExport3D.Enabled = true;
                    btnExport.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                lblStatus.Text = "Traitement en cours...";
                btnExport.Enabled = false;
                Application.DoEvents();

                VisualTopoModel visualTopoModel = demNetService.CreateVisualTopoModelFromFile(visualTopoFile, dataSet, (float)numZFactor.Value);
                string outputFile = Path.GetFullPath(visualTopoModel + ".xlsx");

                demNetService.ExportVisualTopoToExcel(visualTopoModel, outputFile);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{visualTopoModel.Name} - Auteur: {visualTopoModel.Author}");
                sb.AppendLine($"{visualTopoModel.Sets.Count} section(s), {visualTopoModel.Sets.Sum(s => s.Data.Count)} lignes");
                sb.AppendLine($"Profondeur max : {visualTopoModel.Graph.AllNodes.Max(n => n.Model.Depth):N2} m");
                sb.AppendLine($"Distance max : {visualTopoModel.Graph.AllNodes.Max(n => n.Model.DistanceFromEntry):N2} m");
                sb.AppendLine($"Fichier excel exporté vers {outputFile}");

                _excelFile = outputFile;

                txtReport.Text = sb.ToString();

                lblStatus.Text = "Fichier exporté avec succès !";
            }
            catch (Exception ex)
            {
                _excelFile = null;
                lblStatus.Text = $"Erreur : {ex.Message}";
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnExport.Enabled = true;
            }


        }

        private void btnExport3D_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                lblStatus.Text = "Traitement en cours...";
                btnExport3D.Enabled = false;
                Application.DoEvents();

                VisualTopoModel visualTopoModel = demNetService.CreateVisualTopoModelFromFile(visualTopoFile, dataSet, (float)numZFactor.Value);

                string outputFile = Path.GetFullPath($"{visualTopoModel.Name}_{DateTime.Now:yyyy MM dd - HH mm ss}.glb");

                demNetService.ExportVisualTopoToGLB(visualTopoModel, dataSet, outputFile
                                                    , imageryProvider: cmbImagery.SelectedValue as ImageryProvider
                                                    , drawOnTexture: chkDrawOnTexture.Checked
                                                    , marginMeters: (float)numMarginAroundModel.Value
                                                    , zFactor: (float)numZFactor.Value);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{visualTopoModel.Name} - Auteur: {visualTopoModel.Author}");
                sb.AppendLine($"Projection {visualTopoModel.EntryPointProjectionCode}");
                sb.AppendLine($"{visualTopoModel.Sets.Count} section(s), {visualTopoModel.Sets.Sum(s => s.Data.Count)} lignes");
                sb.AppendLine($"Profondeur max : {visualTopoModel.Graph.AllNodes.Max(n => n.Model.Depth):N2} m");
                sb.AppendLine($"Distance max : {visualTopoModel.Graph.AllNodes.Max(n => n.Model.DistanceFromEntry):N2} m");
                sb.AppendLine($"Fichier 3D exporté vers {outputFile}");

                _glbFile = outputFile;

                txtReport.Text = sb.ToString();

                lblStatus.Text = "Fichier exporté avec succès !";
            }
            catch (Exception ex)
            {
                _glbFile = null;
                lblStatus.Text = $"Erreur : {ex.Message}";
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnExport3D.Enabled = true;
            }
        }

        private void btnOpenExcelFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_excelFile))
            {
                Process.Start(_excelFile);
            }
        }

        private void btnOpen3DFile_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(_glbFile))
            {
                Process.Start(_glbFile);
            }
        }
    }
}
