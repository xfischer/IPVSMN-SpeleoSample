﻿using DEM.Net.Core;
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
        VisualTopoModel visualTopoModel = null;
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
            this.Cursor = Cursors.WaitCursor;

            try
            {
                visualTopoModel = File.Exists(fileName) ? demNetService.CreateVisualTopoModelFromFile(fileName, DEMDataSet.AW3D30) : null;

                if (visualTopoModel == null)
                {
                    lblStatus.Text = "Aucun fichier chargé.";
                    btnExport3D.Enabled = false;
                    btnExport.Enabled = false;
                }
                else
                {
                    lblStatus.Text = Path.GetFileName(fileName);
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

                string outputFile = Path.GetFullPath(this.visualTopoModel + ".xlsx");

                demNetService.ExportVisualTopoToExcel(this.visualTopoModel, outputFile);

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

                string outputFile = Path.GetFullPath($"{this.visualTopoModel.Name}_{DateTime.Now:yyyy MM dd - HH mm ss}.glb");

                demNetService.ExportVisualTopoToGLB(this.visualTopoModel, DEMDataSet.AW3D30, outputFile);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{visualTopoModel.Name} - Auteur: {visualTopoModel.Author}");
                sb.AppendLine($"{visualTopoModel.Sets.Count} section(s), {visualTopoModel.Sets.Sum(s => s.Data.Count)} lignes");
                sb.AppendLine($"Profondeur max : {visualTopoModel.Graph.AllNodes.Max(n => n.Model.Depth):N2} m");
                sb.AppendLine($"Distance max : {visualTopoModel.Graph.AllNodes.Max(n => n.Model.DistanceFromEntry):N2} m");
                sb.AppendLine($"Fichier 3D exporté vers {outputFile}");

                txtReport.Text = sb.ToString();

                lblStatus.Text = "Fichier exporté avec succès !";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Erreur : {ex.Message}";
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnExport3D.Enabled = true;
            }
        }
    }
}
