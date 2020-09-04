using DEM.Net.Core;
using DEM.Net.Extension.VisualTopo;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Research.Science.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public frmMain()
        {
            InitializeComponent();

            demNetService = new DemNetVisualTopoService();
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
            visualTopoFile = File.Exists(fileName) ? fileName : null;

            if (visualTopoFile == null)
            {
                lblStatus.Text = "Aucun fichier chargé.";
                btnExport.Enabled = false;
            }
            else
            {
                lblStatus.Text = Path.GetFileName(fileName);
                btnExport.Enabled = true;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Traitement en cours...";
                btnExport.Enabled = false;
                Application.DoEvents();

                string outputFile = Path.GetFullPath(visualTopoFile + ".xlsx");

                var visualTopoModel = demNetService.ExportVisualTopoToExcel(visualTopoFile, outputFile, DEMDataSet.AW3D30);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{visualTopoModel.Name} - Auteur: {visualTopoModel.Author}");
                sb.AppendLine($"{visualTopoModel.Sets.Count} section(s), {visualTopoModel.Sets.Sum(s => s.Data.Count)} lignes");
                sb.AppendLine($"Profondeur max : {visualTopoModel.Graph.AllNodes.Max(n => n.Model.Depth):N2} m");
                sb.AppendLine($"Distance max : {visualTopoModel.Graph.AllNodes.Max(n => n.Model.DistanceFromEntry):N2} m");
                sb.AppendLine($"Fichier excel exporté vers {outputFile}");

                txtReport.Text = sb.ToString();

                lblStatus.Text = "Fichier exporté avec succès !";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Erreur : {ex.Message}";
            }
            finally
            {
                btnExport.Enabled = true;
            }
            

        }
    }
}
