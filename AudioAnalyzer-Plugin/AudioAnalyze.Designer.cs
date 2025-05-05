using LumosControls.Controls;

namespace AudioAnalyzer
{
	partial class audioAnalysForm
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
            if (this.beatTimer != null)
                this.beatTimer.Dispose();
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(audioAnalysForm));
            this.beatBox = new System.Windows.Forms.PictureBox();
            this.levelRBox = new System.Windows.Forms.PictureBox();
            this.levelLBox = new System.Windows.Forms.PictureBox();
            this.spectrumPicture = new System.Windows.Forms.PictureBox();
            this.resetButton = new LumosControls.Controls.LumosButton();
            this.actualBPMLabel = new LumosControls.Controls.LumosLabel();
            this.label13 = new LumosControls.Controls.LumosLabel();
            this.startButton = new LumosControls.Controls.LumosButton();
            this.syncButton = new LumosControls.Controls.LumosButton();
            this.label2 = new LumosControls.Controls.LumosLabel();
            this.label1 = new LumosControls.Controls.LumosLabel();
            this.audioTabControl = new LumosControls.Controls.LumosTabControl();
            this.devicesTabPage = new LumosControls.Controls.LumosTabPage();
            this.gainBar = new LumosControls.Controls.LumosTrackBar();
            this.label8 = new LumosControls.Controls.LumosLabel();
            this.inputsBox = new LumosControls.Controls.LumosComboBox();
            this.label7 = new LumosControls.Controls.LumosLabel();
            this.devicesBox = new LumosControls.Controls.LumosComboBox();
            this.label6 = new LumosControls.Controls.LumosLabel();
            this.vuMeterTabPage = new LumosControls.Controls.LumosTabPage();
            this.lumosLabel1 = new LumosControls.Controls.LumosLabel();
            this.PeakHoldBar = new LumosControls.Controls.LumosTrackBar();
            this.PeakHoldCheckBox = new LumosControls.Controls.LumosCheckBox();
            this.gainVUBar = new LumosControls.Controls.LumosTrackBar();
            this.label3 = new LumosControls.Controls.LumosLabel();
            this.spectrumTabPage = new LumosControls.Controls.LumosTabPage();
            this.infoLabel = new LumosControls.Controls.LumosLabel();
            this.gainSpectrumBar = new LumosControls.Controls.LumosTrackBar();
            this.label10 = new LumosControls.Controls.LumosLabel();
            this.label4 = new LumosControls.Controls.LumosLabel();
            this.subBandBox = new LumosControls.Controls.LumosComboBox();
            this.beatTabPage = new LumosControls.Controls.LumosTabPage();
            this.generatedBpmLabel = new LumosControls.Controls.LumosLabel();
            this.label11 = new LumosControls.Controls.LumosLabel();
            this.halfCheckBox = new LumosControls.Controls.LumosCheckBox();
            this.doubleCheckBox = new LumosControls.Controls.LumosCheckBox();
            this.numberOfBeatsBar = new LumosControls.Controls.LumosTrackBar();
            this.noOfLabel = new LumosControls.Controls.LumosLabel();
            this.sensitivityLabel = new LumosControls.Controls.LumosLabel();
            this.maxBPMBar = new LumosControls.Controls.LumosTrackBar();
            this.sensitivityBar = new LumosControls.Controls.LumosTrackBar();
            this.maxBPMCheckBox = new LumosControls.Controls.LumosCheckBox();
            this.methodBox = new LumosControls.Controls.LumosComboBox();
            this.label5 = new LumosControls.Controls.LumosLabel();
            this.moodTabPage = new LumosControls.Controls.LumosTabPage();
            this.baseToneLabel = new LumosControls.Controls.LumosLabel();
            this.chordLabel = new LumosControls.Controls.LumosLabel();
            this.minorPictureBox = new System.Windows.Forms.PictureBox();
            this.tempoPictureBox = new System.Windows.Forms.PictureBox();
            this.moodPictureBox = new System.Windows.Forms.PictureBox();
            this.label18 = new LumosControls.Controls.LumosLabel();
            this.label19 = new LumosControls.Controls.LumosLabel();
            this.label17 = new LumosControls.Controls.LumosLabel();
            this.label16 = new LumosControls.Controls.LumosLabel();
            this.label15 = new LumosControls.Controls.LumosLabel();
            this.label12 = new LumosControls.Controls.LumosLabel();
            this.generatorTabPage = new LumosControls.Controls.LumosTabPage();
            this.generatorPictureBox = new System.Windows.Forms.PictureBox();
            this.rhythmTypeComboBox = new LumosControls.Controls.LumosComboBox();
            this.label14 = new LumosControls.Controls.LumosLabel();
            this.label9 = new LumosControls.Controls.LumosLabel();
            this.beatGeneratorUpDown = new LumosControls.Controls.LumosNumericUpDown();
            this.activateBeatGeneratorCheckbox = new LumosControls.Controls.LumosCheckBox();
            this.tapButton = new LumosControls.Controls.LumosButton();
            ((System.ComponentModel.ISupportInitialize)(this.beatBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.levelRBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.levelLBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spectrumPicture)).BeginInit();
            this.audioTabControl.SuspendLayout();
            this.devicesTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gainBar)).BeginInit();
            this.vuMeterTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PeakHoldBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gainVUBar)).BeginInit();
            this.spectrumTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gainSpectrumBar)).BeginInit();
            this.beatTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfBeatsBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxBPMBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sensitivityBar)).BeginInit();
            this.moodTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minorPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tempoPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.moodPictureBox)).BeginInit();
            this.generatorTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.generatorPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.beatGeneratorUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // beatBox
            // 
            this.beatBox.BackColor = System.Drawing.Color.Gray;
            this.beatBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.beatBox.Location = new System.Drawing.Point(50, 219);
            this.beatBox.Margin = new System.Windows.Forms.Padding(2);
            this.beatBox.Name = "beatBox";
            this.beatBox.Size = new System.Drawing.Size(22, 17);
            this.beatBox.TabIndex = 10;
            this.beatBox.TabStop = false;
            // 
            // levelRBox
            // 
            this.levelRBox.BackColor = System.Drawing.Color.Gray;
            this.levelRBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.levelRBox.Location = new System.Drawing.Point(50, 198);
            this.levelRBox.Name = "levelRBox";
            this.levelRBox.Size = new System.Drawing.Size(100, 16);
            this.levelRBox.TabIndex = 17;
            this.levelRBox.TabStop = false;
            // 
            // levelLBox
            // 
            this.levelLBox.BackColor = System.Drawing.Color.Gray;
            this.levelLBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.levelLBox.Location = new System.Drawing.Point(50, 176);
            this.levelLBox.Name = "levelLBox";
            this.levelLBox.Size = new System.Drawing.Size(100, 16);
            this.levelLBox.TabIndex = 16;
            this.levelLBox.TabStop = false;
            // 
            // spectrumPicture
            // 
            this.spectrumPicture.BackColor = System.Drawing.Color.Gray;
            this.spectrumPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spectrumPicture.Location = new System.Drawing.Point(166, 176);
            this.spectrumPicture.Name = "spectrumPicture";
            this.spectrumPicture.Size = new System.Drawing.Size(201, 62);
            this.spectrumPicture.TabIndex = 0;
            this.spectrumPicture.TabStop = false;
            // 
            // resetButton
            // 
            this.resetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.resetButton.Image = global::AudioAnalyzer.Properties.Resources.refresh;
            this.resetButton.Location = new System.Drawing.Point(285, 253);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(82, 28);
            this.resetButton.TabIndex = 22;
            this.resetButton.Text = "Reset";
            this.resetButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // actualBPMLabel
            // 
            this.actualBPMLabel.BackColor = System.Drawing.Color.Black;
            this.actualBPMLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.actualBPMLabel.CausesValidation = false;
            this.actualBPMLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.actualBPMLabel.ForeColor = System.Drawing.Color.Lime;
            this.actualBPMLabel.Location = new System.Drawing.Point(77, 219);
            this.actualBPMLabel.Name = "actualBPMLabel";
            this.actualBPMLabel.Size = new System.Drawing.Size(76, 20);
            this.actualBPMLabel.TabIndex = 21;
            this.actualBPMLabel.Text = "0";
            this.actualBPMLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(13, 223);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(31, 16);
            this.label13.TabIndex = 19;
            this.label13.Text = "Beat";
            // 
            // startButton
            // 
            this.startButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.startButton.Image = global::AudioAnalyzer.Properties.Resources.media_play;
            this.startButton.Location = new System.Drawing.Point(11, 253);
            this.startButton.Margin = new System.Windows.Forms.Padding(2);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(186, 28);
            this.startButton.TabIndex = 8;
            this.startButton.Text = "Start";
            this.startButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // syncButton
            // 
            this.syncButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.syncButton.Image = global::AudioAnalyzer.Properties.Resources.media_step_forward;
            this.syncButton.Location = new System.Drawing.Point(203, 253);
            this.syncButton.Name = "syncButton";
            this.syncButton.Size = new System.Drawing.Size(76, 28);
            this.syncButton.TabIndex = 1;
            this.syncButton.Text = "Sync";
            this.syncButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.syncButton.UseVisualStyleBackColor = true;
            this.syncButton.Click += new System.EventHandler(this.syncButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 201);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 16);
            this.label2.TabIndex = 15;
            this.label2.Text = "Right";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 16);
            this.label1.TabIndex = 14;
            this.label1.Text = "Left";
            // 
            // audioTabControl
            // 
            this.audioTabControl.Controls.Add(this.devicesTabPage);
            this.audioTabControl.Controls.Add(this.vuMeterTabPage);
            this.audioTabControl.Controls.Add(this.spectrumTabPage);
            this.audioTabControl.Controls.Add(this.beatTabPage);
            this.audioTabControl.Controls.Add(this.moodTabPage);
            this.audioTabControl.Controls.Add(this.generatorTabPage);
            this.audioTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.audioTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.29091F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.audioTabControl.HighlightColor = System.Drawing.Color.DimGray;
            this.audioTabControl.Location = new System.Drawing.Point(12, 12);
            this.audioTabControl.Name = "audioTabControl";
            this.audioTabControl.SelectedIndex = 0;
            this.audioTabControl.Size = new System.Drawing.Size(362, 158);
            this.audioTabControl.TabIndex = 13;
            // 
            // devicesTabPage
            // 
            this.devicesTabPage.Controls.Add(this.gainBar);
            this.devicesTabPage.Controls.Add(this.label8);
            this.devicesTabPage.Controls.Add(this.inputsBox);
            this.devicesTabPage.Controls.Add(this.label7);
            this.devicesTabPage.Controls.Add(this.devicesBox);
            this.devicesTabPage.Controls.Add(this.label6);
            this.devicesTabPage.Location = new System.Drawing.Point(4, 25);
            this.devicesTabPage.Name = "devicesTabPage";
            this.devicesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.devicesTabPage.Size = new System.Drawing.Size(354, 129);
            this.devicesTabPage.TabIndex = 0;
            this.devicesTabPage.Text = "Devices";
            // 
            // gainBar
            // 
            this.gainBar.AutoSize = false;
            this.gainBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.gainBar.Location = new System.Drawing.Point(59, 77);
            this.gainBar.Maximum = 100;
            this.gainBar.Name = "gainBar";
            this.gainBar.Size = new System.Drawing.Size(286, 27);
            this.gainBar.SmallChange = 5;
            this.gainBar.TabIndex = 11;
            this.gainBar.TickFrequency = 10;
            this.gainBar.Value = 50;
            this.gainBar.Scroll += new System.EventHandler(this.gainBar_Scroll);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 16);
            this.label8.TabIndex = 6;
            this.label8.Text = "Gain :";
            // 
            // inputsBox
            // 
            this.inputsBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.inputsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputsBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.29091F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.inputsBox.FormattingEnabled = true;
            this.inputsBox.Location = new System.Drawing.Point(59, 41);
            this.inputsBox.Name = "inputsBox";
            this.inputsBox.Size = new System.Drawing.Size(286, 22);
            this.inputsBox.TabIndex = 4;
            this.inputsBox.Text = null;
            this.inputsBox.SelectedIndexChanged += new System.EventHandler(this.inputsBox_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 16);
            this.label7.TabIndex = 3;
            this.label7.Text = "Input :";
            // 
            // devicesBox
            // 
            this.devicesBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.devicesBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.devicesBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.29091F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.devicesBox.FormattingEnabled = true;
            this.devicesBox.Location = new System.Drawing.Point(59, 11);
            this.devicesBox.Name = "devicesBox";
            this.devicesBox.Size = new System.Drawing.Size(286, 22);
            this.devicesBox.TabIndex = 1;
            this.devicesBox.Text = null;
            this.devicesBox.SelectedIndexChanged += new System.EventHandler(this.devicesBox_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 16);
            this.label6.TabIndex = 2;
            this.label6.Text = "Device :";
            // 
            // vuMeterTabPage
            // 
            this.vuMeterTabPage.Controls.Add(this.lumosLabel1);
            this.vuMeterTabPage.Controls.Add(this.PeakHoldBar);
            this.vuMeterTabPage.Controls.Add(this.PeakHoldCheckBox);
            this.vuMeterTabPage.Controls.Add(this.gainVUBar);
            this.vuMeterTabPage.Controls.Add(this.label3);
            this.vuMeterTabPage.Location = new System.Drawing.Point(4, 25);
            this.vuMeterTabPage.Name = "vuMeterTabPage";
            this.vuMeterTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.vuMeterTabPage.Size = new System.Drawing.Size(354, 129);
            this.vuMeterTabPage.TabIndex = 1;
            this.vuMeterTabPage.Text = "VU-Meter";
            // 
            // lumosLabel1
            // 
            this.lumosLabel1.AutoSize = true;
            this.lumosLabel1.BackColor = System.Drawing.SystemColors.Control;
            this.lumosLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lumosLabel1.Location = new System.Drawing.Point(118, 92);
            this.lumosLabel1.Name = "lumosLabel1";
            this.lumosLabel1.Size = new System.Drawing.Size(106, 16);
            this.lumosLabel1.TabIndex = 11;
            this.lumosLabel1.Text = "Peak Hold Time :";
            // 
            // PeakHoldBar
            // 
            this.PeakHoldBar.AutoSize = false;
            this.PeakHoldBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.PeakHoldBar.LargeChange = 50;
            this.PeakHoldBar.Location = new System.Drawing.Point(230, 90);
            this.PeakHoldBar.Maximum = 500;
            this.PeakHoldBar.Minimum = 10;
            this.PeakHoldBar.Name = "PeakHoldBar";
            this.PeakHoldBar.Size = new System.Drawing.Size(106, 21);
            this.PeakHoldBar.SmallChange = 10;
            this.PeakHoldBar.TabIndex = 10;
            this.PeakHoldBar.TickFrequency = 50;
            this.PeakHoldBar.Value = 20;
            this.PeakHoldBar.Scroll += new System.EventHandler(this.PeakHoldBar_Scroll);
            // 
            // PeakHoldCheckBox
            // 
            this.PeakHoldCheckBox.AutoSize = true;
            this.PeakHoldCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.29091F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.PeakHoldCheckBox.Location = new System.Drawing.Point(20, 90);
            this.PeakHoldCheckBox.Name = "PeakHoldCheckBox";
            this.PeakHoldCheckBox.Size = new System.Drawing.Size(88, 20);
            this.PeakHoldCheckBox.TabIndex = 9;
            this.PeakHoldCheckBox.Text = "Peak Sink";
            this.PeakHoldCheckBox.UseVisualStyleBackColor = true;
            this.PeakHoldCheckBox.CheckedChanged += new System.EventHandler(this.PeakHoldCheckBox_CheckedChanged);
            // 
            // gainVUBar
            // 
            this.gainVUBar.AutoSize = false;
            this.gainVUBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.gainVUBar.Location = new System.Drawing.Point(9, 43);
            this.gainVUBar.Maximum = 100;
            this.gainVUBar.Name = "gainVUBar";
            this.gainVUBar.Size = new System.Drawing.Size(327, 28);
            this.gainVUBar.SmallChange = 5;
            this.gainVUBar.TabIndex = 6;
            this.gainVUBar.TickFrequency = 10;
            this.gainVUBar.Value = 50;
            this.gainVUBar.Scroll += new System.EventHandler(this.gainVUBar_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Level correction :";
            // 
            // spectrumTabPage
            // 
            this.spectrumTabPage.Controls.Add(this.infoLabel);
            this.spectrumTabPage.Controls.Add(this.gainSpectrumBar);
            this.spectrumTabPage.Controls.Add(this.label10);
            this.spectrumTabPage.Controls.Add(this.label4);
            this.spectrumTabPage.Controls.Add(this.subBandBox);
            this.spectrumTabPage.Location = new System.Drawing.Point(4, 25);
            this.spectrumTabPage.Name = "spectrumTabPage";
            this.spectrumTabPage.Size = new System.Drawing.Size(354, 129);
            this.spectrumTabPage.TabIndex = 2;
            this.spectrumTabPage.Text = "Spectrum";
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.infoLabel.Location = new System.Drawing.Point(277, 14);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(0, 0);
            this.infoLabel.TabIndex = 9;
            // 
            // gainSpectrumBar
            // 
            this.gainSpectrumBar.AutoSize = false;
            this.gainSpectrumBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.gainSpectrumBar.Location = new System.Drawing.Point(15, 74);
            this.gainSpectrumBar.Maximum = 100;
            this.gainSpectrumBar.Name = "gainSpectrumBar";
            this.gainSpectrumBar.Size = new System.Drawing.Size(333, 28);
            this.gainSpectrumBar.SmallChange = 5;
            this.gainSpectrumBar.TabIndex = 8;
            this.gainSpectrumBar.TickFrequency = 10;
            this.gainSpectrumBar.Value = 50;
            this.gainSpectrumBar.Scroll += new System.EventHandler(this.gainSpectrumBar_Scroll);
            this.gainSpectrumBar.ValueChanged += new System.EventHandler(this.gainSpectrumBar_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.Control;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(12, 46);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(106, 16);
            this.label10.TabIndex = 7;
            this.label10.Text = "Level correction :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 16);
            this.label4.TabIndex = 2;
            this.label4.Text = "Number of Bands";
            // 
            // subBandBox
            // 
            this.subBandBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.subBandBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.subBandBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.29091F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.subBandBox.FormattingEnabled = true;
            this.subBandBox.Items.AddRange(new object[] {
            "8",
            "16",
            "32",
            "64",
            "96"});
            this.subBandBox.Location = new System.Drawing.Point(116, 14);
            this.subBandBox.Name = "subBandBox";
            this.subBandBox.Size = new System.Drawing.Size(82, 22);
            this.subBandBox.TabIndex = 1;
            this.subBandBox.Text = null;
            this.subBandBox.SelectedIndexChanged += new System.EventHandler(this.subBandBox_SelectedIndexChanged);
            // 
            // beatTabPage
            // 
            this.beatTabPage.Controls.Add(this.generatedBpmLabel);
            this.beatTabPage.Controls.Add(this.label11);
            this.beatTabPage.Controls.Add(this.halfCheckBox);
            this.beatTabPage.Controls.Add(this.doubleCheckBox);
            this.beatTabPage.Controls.Add(this.numberOfBeatsBar);
            this.beatTabPage.Controls.Add(this.noOfLabel);
            this.beatTabPage.Controls.Add(this.sensitivityLabel);
            this.beatTabPage.Controls.Add(this.maxBPMBar);
            this.beatTabPage.Controls.Add(this.sensitivityBar);
            this.beatTabPage.Controls.Add(this.maxBPMCheckBox);
            this.beatTabPage.Controls.Add(this.methodBox);
            this.beatTabPage.Controls.Add(this.label5);
            this.beatTabPage.Location = new System.Drawing.Point(4, 25);
            this.beatTabPage.Name = "beatTabPage";
            this.beatTabPage.Size = new System.Drawing.Size(354, 129);
            this.beatTabPage.TabIndex = 3;
            this.beatTabPage.Text = "Beat Detection";
            // 
            // generatedBpmLabel
            // 
            this.generatedBpmLabel.AutoSize = true;
            this.generatedBpmLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.generatedBpmLabel.Location = new System.Drawing.Point(298, 105);
            this.generatedBpmLabel.Name = "generatedBpmLabel";
            this.generatedBpmLabel.Size = new System.Drawing.Size(11, 16);
            this.generatedBpmLabel.TabIndex = 18;
            this.generatedBpmLabel.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(195, 106);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 16);
            this.label11.TabIndex = 17;
            this.label11.Text = "generated BPM";
            // 
            // halfCheckBox
            // 
            this.halfCheckBox.AutoSize = true;
            this.halfCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.29091F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.halfCheckBox.Location = new System.Drawing.Point(265, 82);
            this.halfCheckBox.Name = "halfCheckBox";
            this.halfCheckBox.Size = new System.Drawing.Size(48, 20);
            this.halfCheckBox.TabIndex = 15;
            this.halfCheckBox.Text = "half";
            this.halfCheckBox.UseVisualStyleBackColor = true;
            this.halfCheckBox.CheckedChanged += new System.EventHandler(this.halfCheckBox_CheckedChanged);
            // 
            // doubleCheckBox
            // 
            this.doubleCheckBox.AutoSize = true;
            this.doubleCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.29091F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.doubleCheckBox.Location = new System.Drawing.Point(198, 82);
            this.doubleCheckBox.Name = "doubleCheckBox";
            this.doubleCheckBox.Size = new System.Drawing.Size(69, 20);
            this.doubleCheckBox.TabIndex = 14;
            this.doubleCheckBox.Text = "double";
            this.doubleCheckBox.UseVisualStyleBackColor = true;
            this.doubleCheckBox.CheckedChanged += new System.EventHandler(this.doubleCheckBox_CheckedChanged);
            // 
            // numberOfBeatsBar
            // 
            this.numberOfBeatsBar.AutoSize = false;
            this.numberOfBeatsBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.numberOfBeatsBar.Location = new System.Drawing.Point(198, 42);
            this.numberOfBeatsBar.Maximum = 100;
            this.numberOfBeatsBar.Name = "numberOfBeatsBar";
            this.numberOfBeatsBar.Size = new System.Drawing.Size(135, 20);
            this.numberOfBeatsBar.TabIndex = 13;
            this.numberOfBeatsBar.TickFrequency = 10;
            this.numberOfBeatsBar.Value = 16;
            this.numberOfBeatsBar.Scroll += new System.EventHandler(this.numberOfBeatsBar_Scroll);
            this.numberOfBeatsBar.ValueChanged += new System.EventHandler(this.numberOfBeatsBar_Scroll);
            // 
            // noOfLabel
            // 
            this.noOfLabel.AutoSize = true;
            this.noOfLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.noOfLabel.Location = new System.Drawing.Point(195, 22);
            this.noOfLabel.Name = "noOfLabel";
            this.noOfLabel.Size = new System.Drawing.Size(156, 16);
            this.noOfLabel.TabIndex = 12;
            this.noOfLabel.Text = "max. # of additional beats";
            // 
            // sensitivityLabel
            // 
            this.sensitivityLabel.AutoSize = true;
            this.sensitivityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.sensitivityLabel.Location = new System.Drawing.Point(14, 46);
            this.sensitivityLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.sensitivityLabel.Name = "sensitivityLabel";
            this.sensitivityLabel.Size = new System.Drawing.Size(64, 16);
            this.sensitivityLabel.TabIndex = 8;
            this.sensitivityLabel.Text = "Sensitivity";
            // 
            // maxBPMBar
            // 
            this.maxBPMBar.AutoSize = false;
            this.maxBPMBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.maxBPMBar.Location = new System.Drawing.Point(17, 99);
            this.maxBPMBar.Maximum = 330;
            this.maxBPMBar.Minimum = 30;
            this.maxBPMBar.Name = "maxBPMBar";
            this.maxBPMBar.Size = new System.Drawing.Size(148, 20);
            this.maxBPMBar.SmallChange = 5;
            this.maxBPMBar.TabIndex = 10;
            this.maxBPMBar.TickFrequency = 20;
            this.maxBPMBar.Value = 240;
            this.maxBPMBar.Scroll += new System.EventHandler(this.maxBPMBar_Scroll);
            // 
            // sensitivityBar
            // 
            this.sensitivityBar.AutoSize = false;
            this.sensitivityBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.sensitivityBar.Location = new System.Drawing.Point(72, 40);
            this.sensitivityBar.Margin = new System.Windows.Forms.Padding(2);
            this.sensitivityBar.Maximum = 100;
            this.sensitivityBar.Name = "sensitivityBar";
            this.sensitivityBar.Size = new System.Drawing.Size(93, 22);
            this.sensitivityBar.SmallChange = 5;
            this.sensitivityBar.TabIndex = 7;
            this.sensitivityBar.TickFrequency = 10;
            this.sensitivityBar.Value = 50;
            this.sensitivityBar.Scroll += new System.EventHandler(this.sensitivityBar_Scroll);
            // 
            // maxBPMCheckBox
            // 
            this.maxBPMCheckBox.AutoSize = true;
            this.maxBPMCheckBox.Checked = true;
            this.maxBPMCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.maxBPMCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.29091F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.maxBPMCheckBox.Location = new System.Drawing.Point(17, 76);
            this.maxBPMCheckBox.Name = "maxBPMCheckBox";
            this.maxBPMCheckBox.Size = new System.Drawing.Size(118, 20);
            this.maxBPMCheckBox.TabIndex = 8;
            this.maxBPMCheckBox.Text = "Max BPM = 240";
            this.maxBPMCheckBox.UseVisualStyleBackColor = true;
            this.maxBPMCheckBox.CheckedChanged += new System.EventHandler(this.maxBPMCheckBox_CheckedChanged);
            // 
            // methodBox
            // 
            this.methodBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.methodBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.methodBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.29091F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.methodBox.FormattingEnabled = true;
            this.methodBox.Items.AddRange(new object[] {
            "Standard weight method",
            "Simple sound energy",
            "Frequency selected sound energy",
            "Automatic"});
            this.methodBox.Location = new System.Drawing.Point(76, 14);
            this.methodBox.Name = "methodBox";
            this.methodBox.Size = new System.Drawing.Size(89, 22);
            this.methodBox.TabIndex = 0;
            this.methodBox.Text = null;
            this.methodBox.SelectedIndexChanged += new System.EventHandler(this.methodBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(14, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 16);
            this.label5.TabIndex = 1;
            this.label5.Text = "Algorithm :";
            // 
            // moodTabPage
            // 
            this.moodTabPage.Controls.Add(this.baseToneLabel);
            this.moodTabPage.Controls.Add(this.chordLabel);
            this.moodTabPage.Controls.Add(this.minorPictureBox);
            this.moodTabPage.Controls.Add(this.tempoPictureBox);
            this.moodTabPage.Controls.Add(this.moodPictureBox);
            this.moodTabPage.Controls.Add(this.label18);
            this.moodTabPage.Controls.Add(this.label19);
            this.moodTabPage.Controls.Add(this.label17);
            this.moodTabPage.Controls.Add(this.label16);
            this.moodTabPage.Controls.Add(this.label15);
            this.moodTabPage.Controls.Add(this.label12);
            this.moodTabPage.Location = new System.Drawing.Point(4, 25);
            this.moodTabPage.Name = "moodTabPage";
            this.moodTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.moodTabPage.Size = new System.Drawing.Size(354, 129);
            this.moodTabPage.TabIndex = 6;
            this.moodTabPage.Text = "Mood";
            // 
            // baseToneLabel
            // 
            this.baseToneLabel.AutoSize = true;
            this.baseToneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.baseToneLabel.Location = new System.Drawing.Point(66, 6);
            this.baseToneLabel.Name = "baseToneLabel";
            this.baseToneLabel.Size = new System.Drawing.Size(9, 16);
            this.baseToneLabel.TabIndex = 11;
            this.baseToneLabel.Text = "-";
            // 
            // chordLabel
            // 
            this.chordLabel.AutoSize = true;
            this.chordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.chordLabel.Location = new System.Drawing.Point(66, 26);
            this.chordLabel.Name = "chordLabel";
            this.chordLabel.Size = new System.Drawing.Size(9, 16);
            this.chordLabel.TabIndex = 10;
            this.chordLabel.Text = "-";
            // 
            // minorPictureBox
            // 
            this.minorPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.minorPictureBox.Location = new System.Drawing.Point(11, 88);
            this.minorPictureBox.Name = "minorPictureBox";
            this.minorPictureBox.Size = new System.Drawing.Size(123, 16);
            this.minorPictureBox.TabIndex = 9;
            this.minorPictureBox.TabStop = false;
            // 
            // tempoPictureBox
            // 
            this.tempoPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tempoPictureBox.Location = new System.Drawing.Point(11, 51);
            this.tempoPictureBox.Name = "tempoPictureBox";
            this.tempoPictureBox.Size = new System.Drawing.Size(123, 15);
            this.tempoPictureBox.TabIndex = 8;
            this.tempoPictureBox.TabStop = false;
            // 
            // moodPictureBox
            // 
            this.moodPictureBox.BackColor = System.Drawing.Color.Gray;
            this.moodPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.moodPictureBox.Location = new System.Drawing.Point(150, 6);
            this.moodPictureBox.Name = "moodPictureBox";
            this.moodPictureBox.Size = new System.Drawing.Size(198, 113);
            this.moodPictureBox.TabIndex = 7;
            this.moodPictureBox.TabStop = false;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(98, 107);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(40, 16);
            this.label18.TabIndex = 6;
            this.label18.Text = "happy";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(8, 107);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(25, 16);
            this.label19.TabIndex = 5;
            this.label19.Text = "sad";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(93, 69);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(47, 16);
            this.label17.TabIndex = 4;
            this.label17.Text = "speedy";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(8, 69);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(40, 16);
            this.label16.TabIndex = 3;
            this.label16.Text = "slowly";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(8, 26);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 16);
            this.label15.TabIndex = 2;
            this.label15.Text = "Chord";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(8, 6);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 16);
            this.label12.TabIndex = 1;
            this.label12.Text = "Basetone";
            // 
            // generatorTabPage
            // 
            this.generatorTabPage.Controls.Add(this.generatorPictureBox);
            this.generatorTabPage.Controls.Add(this.rhythmTypeComboBox);
            this.generatorTabPage.Controls.Add(this.label14);
            this.generatorTabPage.Controls.Add(this.label9);
            this.generatorTabPage.Controls.Add(this.beatGeneratorUpDown);
            this.generatorTabPage.Controls.Add(this.activateBeatGeneratorCheckbox);
            this.generatorTabPage.Controls.Add(this.tapButton);
            this.generatorTabPage.Location = new System.Drawing.Point(4, 25);
            this.generatorTabPage.Name = "generatorTabPage";
            this.generatorTabPage.Size = new System.Drawing.Size(354, 129);
            this.generatorTabPage.TabIndex = 5;
            this.generatorTabPage.Text = "Beat Generator";
            // 
            // generatorPictureBox
            // 
            this.generatorPictureBox.Location = new System.Drawing.Point(22, 62);
            this.generatorPictureBox.Name = "generatorPictureBox";
            this.generatorPictureBox.Size = new System.Drawing.Size(303, 26);
            this.generatorPictureBox.TabIndex = 7;
            this.generatorPictureBox.TabStop = false;
            // 
            // rhythmTypeComboBox
            // 
            this.rhythmTypeComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.rhythmTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rhythmTypeComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.29091F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.rhythmTypeComboBox.FormattingEnabled = true;
            this.rhythmTypeComboBox.Location = new System.Drawing.Point(187, 34);
            this.rhythmTypeComboBox.Name = "rhythmTypeComboBox";
            this.rhythmTypeComboBox.Size = new System.Drawing.Size(151, 22);
            this.rhythmTypeComboBox.TabIndex = 6;
            this.rhythmTypeComboBox.Text = null;
            this.rhythmTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.rhythmTypeComboBox_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(135, 37);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 16);
            this.label14.TabIndex = 5;
            this.label14.Text = "Rhythm:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(3, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 16);
            this.label9.TabIndex = 4;
            this.label9.Text = "BPM:";
            // 
            // beatGeneratorUpDown
            // 
            this.beatGeneratorUpDown.DecimalPlaces = 0;
            this.beatGeneratorUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.beatGeneratorUpDown.Hexadecimal = false;
            this.beatGeneratorUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.beatGeneratorUpDown.Location = new System.Drawing.Point(42, 35);
            this.beatGeneratorUpDown.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.beatGeneratorUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.beatGeneratorUpDown.Name = "beatGeneratorUpDown";
            this.beatGeneratorUpDown.Size = new System.Drawing.Size(65, 20);
            this.beatGeneratorUpDown.TabIndex = 3;
            this.beatGeneratorUpDown.Text = "120";
            this.beatGeneratorUpDown.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.beatGeneratorUpDown.ValueChanged += new System.EventHandler(this.beatGeneratorUpDown_ValueChanged);
            // 
            // activateBeatGeneratorCheckbox
            // 
            this.activateBeatGeneratorCheckbox.AutoSize = true;
            this.activateBeatGeneratorCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.29091F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.activateBeatGeneratorCheckbox.Location = new System.Drawing.Point(6, 11);
            this.activateBeatGeneratorCheckbox.Name = "activateBeatGeneratorCheckbox";
            this.activateBeatGeneratorCheckbox.Size = new System.Drawing.Size(319, 20);
            this.activateBeatGeneratorCheckbox.TabIndex = 2;
            this.activateBeatGeneratorCheckbox.Text = "activate Beat Generator (disables beat detection)";
            this.activateBeatGeneratorCheckbox.UseVisualStyleBackColor = true;
            this.activateBeatGeneratorCheckbox.CheckedChanged += new System.EventHandler(this.activateBeatGeneratorCheckbox_CheckedChanged);
            // 
            // tapButton
            // 
            this.tapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tapButton.Image = global::AudioAnalyzer.Properties.Resources.step;
            this.tapButton.Location = new System.Drawing.Point(6, 94);
            this.tapButton.Name = "tapButton";
            this.tapButton.Size = new System.Drawing.Size(332, 35);
            this.tapButton.TabIndex = 0;
            this.tapButton.Text = "&Tap";
            this.tapButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.tapButton.UseVisualStyleBackColor = true;
            this.tapButton.Click += new System.EventHandler(this.tapButton_Click);
            // 
            // audioAnalysForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 289);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.actualBPMLabel);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.beatBox);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.levelRBox);
            this.Controls.Add(this.syncButton);
            this.Controls.Add(this.levelLBox);
            this.Controls.Add(this.spectrumPicture);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.audioTabControl);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.MainFormMenu = LumosLIB.GUI.Windows.MenuType.Windows;
            this.MenuIconKey = "window_equalizer";
            this.MinimizeIcon = "";
            this.Name = "audioAnalysForm";
            this.TabText = "Audio Analyzer";
            this.Text = "Audio Analyzer";
            this.Load += new System.EventHandler(this.audioAnalysForm_Load);
            this.Shown += new System.EventHandler(this.audioAnalysForm_Shown);
            this.Resize += new System.EventHandler(this.audioAnalysForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.beatBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.levelRBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.levelLBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spectrumPicture)).EndInit();
            this.audioTabControl.ResumeLayout(false);
            this.devicesTabPage.ResumeLayout(false);
            this.devicesTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gainBar)).EndInit();
            this.vuMeterTabPage.ResumeLayout(false);
            this.vuMeterTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PeakHoldBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gainVUBar)).EndInit();
            this.spectrumTabPage.ResumeLayout(false);
            this.spectrumTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gainSpectrumBar)).EndInit();
            this.beatTabPage.ResumeLayout(false);
            this.beatTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfBeatsBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxBPMBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sensitivityBar)).EndInit();
            this.moodTabPage.ResumeLayout(false);
            this.moodTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minorPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tempoPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.moodPictureBox)).EndInit();
            this.generatorTabPage.ResumeLayout(false);
            this.generatorTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.generatorPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beatGeneratorUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private LumosLabel label3;
        private LumosLabel label4;
		private LumosComboBox inputsBox;
		private LumosLabel label7;
		private LumosLabel label6;
        private LumosComboBox devicesBox;
        private LumosLabel label5;
        private LumosLabel label8;
		private System.Windows.Forms.PictureBox beatBox;
        private LumosButton startButton;
        private LumosLabel sensitivityLabel;
        protected internal System.Windows.Forms.PictureBox spectrumPicture;
        private LumosTabControl audioTabControl;
        private LumosTabPage devicesTabPage;
        private LumosTabPage vuMeterTabPage;
        private LumosTabPage spectrumTabPage;
        private LumosTabPage beatTabPage;
        protected internal System.Windows.Forms.PictureBox levelRBox;
        protected internal System.Windows.Forms.PictureBox levelLBox;
        private LumosLabel label2;
        private LumosLabel label1;
        private LumosLabel label13;
        private LumosTabPage generatorTabPage;
        private LumosButton tapButton;
        private LumosButton syncButton;
        private LumosLabel label9;
        private LumosLabel label14;
        private System.Windows.Forms.PictureBox generatorPictureBox;
        private LumosLabel noOfLabel;
        private LumosLabel label11;
        private LumosLabel actualBPMLabel;
        private LumosLabel generatedBpmLabel;
        private LumosButton resetButton;
        private LumosLabel label10;
        private LumosTabPage moodTabPage;
        private LumosLabel label17;
        private LumosLabel label16;
        private LumosLabel label15;
        private LumosLabel label12;
        private System.Windows.Forms.PictureBox minorPictureBox;
        private System.Windows.Forms.PictureBox tempoPictureBox;
        private System.Windows.Forms.PictureBox moodPictureBox;
        private LumosLabel label18;
        private LumosLabel label19;
        private LumosLabel baseToneLabel;
        private LumosLabel chordLabel;
        private LumosLabel infoLabel;
        public LumosTrackBar gainBar;
        public LumosComboBox subBandBox;
        public LumosComboBox methodBox;
        public LumosTrackBar gainVUBar;
        public LumosTrackBar gainSpectrumBar;
        public LumosCheckBox maxBPMCheckBox;
        public LumosTrackBar numberOfBeatsBar;
        public LumosCheckBox activateBeatGeneratorCheckbox;
        public LumosNumericUpDown beatGeneratorUpDown;
        public LumosComboBox rhythmTypeComboBox;
        public LumosTrackBar sensitivityBar;
        public LumosTrackBar maxBPMBar;
        public LumosCheckBox halfCheckBox;
        public LumosCheckBox doubleCheckBox;
        public LumosCheckBox PeakHoldCheckBox;
        public LumosTrackBar PeakHoldBar;
        private LumosLabel lumosLabel1;
    }
}

