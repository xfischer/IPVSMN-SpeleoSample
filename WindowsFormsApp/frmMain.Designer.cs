﻿namespace WindowsFormsApp
{
    partial class frmMain
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOpenVTopoFile = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.txtReport = new System.Windows.Forms.TextBox();
            this.btnExport3D = new System.Windows.Forms.Button();
            this.btnOpenExcelFile = new System.Windows.Forms.Button();
            this.btnOpen3DFile = new System.Windows.Forms.Button();
            this.chkDrawOnTexture = new System.Windows.Forms.CheckBox();
            this.numMarginAroundModel = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginAroundModel)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOpenVTopoFile
            // 
            this.btnOpenVTopoFile.Location = new System.Drawing.Point(12, 17);
            this.btnOpenVTopoFile.Name = "btnOpenVTopoFile";
            this.btnOpenVTopoFile.Size = new System.Drawing.Size(204, 23);
            this.btnOpenVTopoFile.TabIndex = 0;
            this.btnOpenVTopoFile.Text = "Ouvrir un fichier VisualTopo...";
            this.btnOpenVTopoFile.UseVisualStyleBackColor = true;
            this.btnOpenVTopoFile.Click += new System.EventHandler(this.btnOpenVTopoFile_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(222, 22);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(109, 13);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Aucun fichier chargé.";
            // 
            // btnExport
            // 
            this.btnExport.Enabled = false;
            this.btnExport.Location = new System.Drawing.Point(12, 60);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(134, 23);
            this.btnExport.TabIndex = 0;
            this.btnExport.Text = "Exporter vers Excel";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // txtReport
            // 
            this.txtReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReport.Location = new System.Drawing.Point(225, 62);
            this.txtReport.Multiline = true;
            this.txtReport.Name = "txtReport";
            this.txtReport.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReport.Size = new System.Drawing.Size(397, 275);
            this.txtReport.TabIndex = 2;
            // 
            // btnExport3D
            // 
            this.btnExport3D.Enabled = false;
            this.btnExport3D.Location = new System.Drawing.Point(12, 89);
            this.btnExport3D.Name = "btnExport3D";
            this.btnExport3D.Size = new System.Drawing.Size(134, 23);
            this.btnExport3D.TabIndex = 3;
            this.btnExport3D.Text = "Exporter en 3D";
            this.btnExport3D.UseVisualStyleBackColor = true;
            this.btnExport3D.Click += new System.EventHandler(this.btnExport3D_Click);
            // 
            // btnOpenExcelFile
            // 
            this.btnOpenExcelFile.Location = new System.Drawing.Point(152, 60);
            this.btnOpenExcelFile.Name = "btnOpenExcelFile";
            this.btnOpenExcelFile.Size = new System.Drawing.Size(46, 23);
            this.btnOpenExcelFile.TabIndex = 4;
            this.btnOpenExcelFile.Text = "Ouvrir le dernier fichier généré";
            this.btnOpenExcelFile.UseVisualStyleBackColor = true;
            this.btnOpenExcelFile.Click += new System.EventHandler(this.btnOpenExcelFile_Click);
            // 
            // btnOpen3DFile
            // 
            this.btnOpen3DFile.Location = new System.Drawing.Point(152, 89);
            this.btnOpen3DFile.Name = "btnOpen3DFile";
            this.btnOpen3DFile.Size = new System.Drawing.Size(46, 23);
            this.btnOpen3DFile.TabIndex = 5;
            this.btnOpen3DFile.Text = "Ouvrir le dernier fichier généré";
            this.btnOpen3DFile.UseVisualStyleBackColor = true;
            this.btnOpen3DFile.Click += new System.EventHandler(this.btnOpen3DFile_Click);
            // 
            // chkDrawOnTexture
            // 
            this.chkDrawOnTexture.AutoSize = true;
            this.chkDrawOnTexture.Location = new System.Drawing.Point(12, 118);
            this.chkDrawOnTexture.Name = "chkDrawOnTexture";
            this.chkDrawOnTexture.Size = new System.Drawing.Size(125, 17);
            this.chkDrawOnTexture.TabIndex = 6;
            this.chkDrawOnTexture.Text = "Cavité sur la texture";
            this.chkDrawOnTexture.UseVisualStyleBackColor = true;
            // 
            // numMarginAroundModel
            // 
            this.numMarginAroundModel.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numMarginAroundModel.Location = new System.Drawing.Point(74, 141);
            this.numMarginAroundModel.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numMarginAroundModel.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numMarginAroundModel.Name = "numMarginAroundModel";
            this.numMarginAroundModel.Size = new System.Drawing.Size(114, 21);
            this.numMarginAroundModel.TabIndex = 7;
            this.numMarginAroundModel.ThousandsSeparator = true;
            this.numMarginAroundModel.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Marge (m)";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 349);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numMarginAroundModel);
            this.Controls.Add(this.chkDrawOnTexture);
            this.Controls.Add(this.btnOpen3DFile);
            this.Controls.Add(this.btnOpenExcelFile);
            this.Controls.Add(this.btnExport3D);
            this.Controls.Add(this.txtReport);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnOpenVTopoFile);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Name = "frmMain";
            this.Text = "IPVSMN Sample avec DEM Net";
            ((System.ComponentModel.ISupportInitialize)(this.numMarginAroundModel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOpenVTopoFile;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.TextBox txtReport;
        private System.Windows.Forms.Button btnExport3D;
        private System.Windows.Forms.Button btnOpenExcelFile;
        private System.Windows.Forms.Button btnOpen3DFile;
        private System.Windows.Forms.CheckBox chkDrawOnTexture;
        private System.Windows.Forms.NumericUpDown numMarginAroundModel;
        private System.Windows.Forms.Label label1;
    }
}

