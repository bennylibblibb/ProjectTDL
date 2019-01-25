using System.IO;

namespace DataOfScouts
{
    partial class DataOfScouts
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        { 
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataOfScouts));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpAuthorization = new System.Windows.Forms.TabPage();
            this.txtXmlFileUrl = new System.Windows.Forms.TextBox();
            this.btnXmlParser = new System.Windows.Forms.Button();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.lstStatus = new System.Windows.Forms.ListBox();
            this.gbAuthorization = new System.Windows.Forms.GroupBox();
            this.lbResults = new System.Windows.Forms.Label();
            this.lbToken = new System.Windows.Forms.Label();
            this.lbAuthorization = new System.Windows.Forms.Label();
            this.tpAreas = new System.Windows.Forms.TabPage();
            this.dgvAreas = new System.Windows.Forms.DataGridView();
            this.tpCompetitions = new System.Windows.Forms.TabPage();
            this.dgvComp = new System.Windows.Forms.DataGridView();
            this.tpSeasons = new System.Windows.Forms.TabPage();
            this.dgvSeasons = new System.Windows.Forms.DataGridView();
            this.tpStages = new System.Windows.Forms.TabPage();
            this.dgvStages = new System.Windows.Forms.DataGridView();
            this.tpGroups = new System.Windows.Forms.TabPage();
            this.dgvGroups = new System.Windows.Forms.DataGridView();
            this.tpTeam = new System.Windows.Forms.TabPage();
            this.dgvTeam = new System.Windows.Forms.DataGridView();
            this.tpPlayer = new System.Windows.Forms.TabPage();
            this.dgvPlayer = new System.Windows.Forms.DataGridView();
            this.tpEvent = new System.Windows.Forms.TabPage();
            this.dgvEvent = new MasterDetailSample.MasterControl();
            this.tpBook = new System.Windows.Forms.TabPage();
            this.dgvBookedEvent = new MasterDetailSample.MasterControl();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.bnAreas = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.tsbPrevious = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.nbAreasCurrent = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.nbAreasPage = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbNext = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.nbAreasTotal = new System.Windows.Forms.ToolStripLabel();
            this.tsbGet2 = new System.Windows.Forms.ToolStripButton();
            this.bnAreas2 = new System.Windows.Forms.BindingNavigator(this.components);
            this.tslArea = new System.Windows.Forms.ToolStripLabel();
            this.tsbArea = new System.Windows.Forms.ToolStripTextBox();
            this.tsdAreaParentId = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsdArea = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsdComp = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsdSeason = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsdStage = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsdGroup = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsdPartic = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsdEvent = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbGet = new System.Windows.Forms.ToolStripButton();
            this.cmsBook = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miBook = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsBooked = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tabControl1.SuspendLayout();
            this.tpAuthorization.SuspendLayout();
            this.gbAuthorization.SuspendLayout();
            this.tpAreas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAreas)).BeginInit();
            this.tpCompetitions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComp)).BeginInit();
            this.tpSeasons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeasons)).BeginInit();
            this.tpStages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStages)).BeginInit();
            this.tpGroups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).BeginInit();
            this.tpTeam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTeam)).BeginInit();
            this.tpPlayer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayer)).BeginInit();
            this.tpEvent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEvent)).BeginInit();
            this.tpBook.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBookedEvent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnAreas)).BeginInit();
            this.bnAreas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnAreas2)).BeginInit();
            this.bnAreas2.SuspendLayout();
            this.cmsBook.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tpAuthorization);
            this.tabControl1.Controls.Add(this.tpAreas);
            this.tabControl1.Controls.Add(this.tpCompetitions);
            this.tabControl1.Controls.Add(this.tpSeasons);
            this.tabControl1.Controls.Add(this.tpStages);
            this.tabControl1.Controls.Add(this.tpGroups);
            this.tabControl1.Controls.Add(this.tpTeam);
            this.tabControl1.Controls.Add(this.tpPlayer);
            this.tabControl1.Controls.Add(this.tpEvent);
            this.tabControl1.Controls.Add(this.tpBook);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tabControl1.Location = new System.Drawing.Point(0, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(972, 539);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tpAuthorization
            // 
            this.tpAuthorization.Controls.Add(this.txtXmlFileUrl);
            this.tpAuthorization.Controls.Add(this.btnXmlParser);
            this.tpAuthorization.Controls.Add(this.btnSelectFile);
            this.tpAuthorization.Controls.Add(this.lstStatus);
            this.tpAuthorization.Controls.Add(this.gbAuthorization);
            this.tpAuthorization.Location = new System.Drawing.Point(4, 29);
            this.tpAuthorization.Name = "tpAuthorization";
            this.tpAuthorization.Size = new System.Drawing.Size(964, 506);
            this.tpAuthorization.TabIndex = 5;
            this.tpAuthorization.Text = "Authorization";
            this.tpAuthorization.UseVisualStyleBackColor = true;
            // 
            // txtXmlFileUrl
            // 
            this.txtXmlFileUrl.Location = new System.Drawing.Point(886, 3);
            this.txtXmlFileUrl.Name = "txtXmlFileUrl";
            this.txtXmlFileUrl.Size = new System.Drawing.Size(75, 26);
            this.txtXmlFileUrl.TabIndex = 4;
            // 
            // btnXmlParser
            // 
            this.btnXmlParser.Location = new System.Drawing.Point(886, 64);
            this.btnXmlParser.Name = "btnXmlParser";
            this.btnXmlParser.Size = new System.Drawing.Size(75, 31);
            this.btnXmlParser.TabIndex = 3;
            this.btnXmlParser.Text = "Done";
            this.btnXmlParser.UseVisualStyleBackColor = true;
            this.btnXmlParser.Click += new System.EventHandler(this.btnXmlParser_Click);
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(886, 29);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(75, 29);
            this.btnSelectFile.TabIndex = 2;
            this.btnSelectFile.Text = "Select";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // lstStatus
            // 
            this.lstStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstStatus.FormattingEnabled = true;
            this.lstStatus.ItemHeight = 20;
            this.lstStatus.Location = new System.Drawing.Point(3, 3);
            this.lstStatus.Name = "lstStatus";
            this.lstStatus.Size = new System.Drawing.Size(877, 484);
            this.lstStatus.TabIndex = 1;
            // 
            // gbAuthorization
            // 
            this.gbAuthorization.Controls.Add(this.lbResults);
            this.gbAuthorization.Controls.Add(this.lbToken);
            this.gbAuthorization.Controls.Add(this.lbAuthorization);
            this.gbAuthorization.Location = new System.Drawing.Point(785, 200);
            this.gbAuthorization.Name = "gbAuthorization";
            this.gbAuthorization.Size = new System.Drawing.Size(144, 99);
            this.gbAuthorization.TabIndex = 0;
            this.gbAuthorization.TabStop = false;
            this.gbAuthorization.Visible = false;
            // 
            // lbResults
            // 
            this.lbResults.Location = new System.Drawing.Point(116, 32);
            this.lbResults.Name = "lbResults";
            this.lbResults.Size = new System.Drawing.Size(338, 86);
            this.lbResults.TabIndex = 3;
            // 
            // lbToken
            // 
            this.lbToken.Location = new System.Drawing.Point(22, 59);
            this.lbToken.Name = "lbToken";
            this.lbToken.Size = new System.Drawing.Size(215, 20);
            this.lbToken.TabIndex = 1;
            // 
            // lbAuthorization
            // 
            this.lbAuthorization.Location = new System.Drawing.Point(22, 32);
            this.lbAuthorization.Name = "lbAuthorization";
            this.lbAuthorization.Size = new System.Drawing.Size(88, 20);
            this.lbAuthorization.TabIndex = 0;
            // 
            // tpAreas
            // 
            this.tpAreas.Controls.Add(this.dgvAreas);
            this.tpAreas.Location = new System.Drawing.Point(4, 29);
            this.tpAreas.Name = "tpAreas";
            this.tpAreas.Size = new System.Drawing.Size(964, 506);
            this.tpAreas.TabIndex = 0;
            this.tpAreas.Text = "areas";
            this.tpAreas.UseVisualStyleBackColor = true;
            // 
            // dgvAreas
            // 
            this.dgvAreas.AllowUserToOrderColumns = true;
            this.dgvAreas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAreas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvAreas.Location = new System.Drawing.Point(3, 3);
            this.dgvAreas.Name = "dgvAreas";
            this.dgvAreas.Size = new System.Drawing.Size(958, 500);
            this.dgvAreas.TabIndex = 0;
            // 
            // tpCompetitions
            // 
            this.tpCompetitions.Controls.Add(this.dgvComp);
            this.tpCompetitions.Location = new System.Drawing.Point(4, 29);
            this.tpCompetitions.Name = "tpCompetitions";
            this.tpCompetitions.Size = new System.Drawing.Size(964, 506);
            this.tpCompetitions.TabIndex = 0;
            this.tpCompetitions.Text = "competitions";
            this.tpCompetitions.UseVisualStyleBackColor = true;
            // 
            // dgvComp
            // 
            this.dgvComp.AllowUserToOrderColumns = true;
            this.dgvComp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvComp.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvComp.Location = new System.Drawing.Point(3, 3);
            this.dgvComp.Name = "dgvComp";
            this.dgvComp.Size = new System.Drawing.Size(958, 500);
            this.dgvComp.TabIndex = 1;
            // 
            // tpSeasons
            // 
            this.tpSeasons.Controls.Add(this.dgvSeasons);
            this.tpSeasons.Location = new System.Drawing.Point(4, 29);
            this.tpSeasons.Name = "tpSeasons";
            this.tpSeasons.Padding = new System.Windows.Forms.Padding(3);
            this.tpSeasons.Size = new System.Drawing.Size(964, 506);
            this.tpSeasons.TabIndex = 1;
            this.tpSeasons.Text = "seasons";
            this.tpSeasons.UseVisualStyleBackColor = true;
            // 
            // dgvSeasons
            // 
            this.dgvSeasons.AllowUserToOrderColumns = true;
            this.dgvSeasons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSeasons.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvSeasons.Location = new System.Drawing.Point(3, 3);
            this.dgvSeasons.Name = "dgvSeasons";
            this.dgvSeasons.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.dgvSeasons.Size = new System.Drawing.Size(958, 500);
            this.dgvSeasons.TabIndex = 1;
            // 
            // tpStages
            // 
            this.tpStages.Controls.Add(this.dgvStages);
            this.tpStages.Location = new System.Drawing.Point(4, 29);
            this.tpStages.Name = "tpStages";
            this.tpStages.Padding = new System.Windows.Forms.Padding(3);
            this.tpStages.Size = new System.Drawing.Size(964, 506);
            this.tpStages.TabIndex = 2;
            this.tpStages.Text = "stages";
            this.tpStages.UseVisualStyleBackColor = true;
            // 
            // dgvStages
            // 
            this.dgvStages.AllowUserToOrderColumns = true;
            this.dgvStages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStages.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvStages.Location = new System.Drawing.Point(3, 3);
            this.dgvStages.Name = "dgvStages";
            this.dgvStages.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.dgvStages.Size = new System.Drawing.Size(958, 500);
            this.dgvStages.TabIndex = 2;
            // 
            // tpGroups
            // 
            this.tpGroups.Controls.Add(this.dgvGroups);
            this.tpGroups.Location = new System.Drawing.Point(4, 29);
            this.tpGroups.Name = "tpGroups";
            this.tpGroups.Padding = new System.Windows.Forms.Padding(3);
            this.tpGroups.Size = new System.Drawing.Size(964, 506);
            this.tpGroups.TabIndex = 3;
            this.tpGroups.Text = "groups";
            this.tpGroups.UseVisualStyleBackColor = true;
            // 
            // dgvGroups
            // 
            this.dgvGroups.AllowUserToOrderColumns = true;
            this.dgvGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvGroups.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvGroups.Location = new System.Drawing.Point(3, 3);
            this.dgvGroups.Name = "dgvGroups";
            this.dgvGroups.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.dgvGroups.Size = new System.Drawing.Size(958, 500);
            this.dgvGroups.TabIndex = 2;
            // 
            // tpTeam
            // 
            this.tpTeam.Controls.Add(this.dgvTeam);
            this.tpTeam.Location = new System.Drawing.Point(4, 29);
            this.tpTeam.Name = "tpTeam";
            this.tpTeam.Size = new System.Drawing.Size(964, 506);
            this.tpTeam.TabIndex = 7;
            this.tpTeam.Text = "team";
            this.tpTeam.UseVisualStyleBackColor = true;
            // 
            // dgvTeam
            // 
            this.dgvTeam.AllowUserToOrderColumns = true;
            this.dgvTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTeam.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvTeam.Location = new System.Drawing.Point(3, 3);
            this.dgvTeam.Name = "dgvTeam";
            this.dgvTeam.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.dgvTeam.Size = new System.Drawing.Size(958, 500);
            this.dgvTeam.TabIndex = 3;
            // 
            // tpPlayer
            // 
            this.tpPlayer.Controls.Add(this.dgvPlayer);
            this.tpPlayer.Location = new System.Drawing.Point(4, 29);
            this.tpPlayer.Name = "tpPlayer";
            this.tpPlayer.Size = new System.Drawing.Size(964, 506);
            this.tpPlayer.TabIndex = 8;
            this.tpPlayer.Text = "player";
            this.tpPlayer.UseVisualStyleBackColor = true;
            // 
            // dgvPlayer
            // 
            this.dgvPlayer.AllowUserToOrderColumns = true;
            this.dgvPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPlayer.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvPlayer.Location = new System.Drawing.Point(3, 3);
            this.dgvPlayer.Name = "dgvPlayer";
            this.dgvPlayer.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.dgvPlayer.Size = new System.Drawing.Size(958, 500);
            this.dgvPlayer.TabIndex = 3;
            // 
            // tpEvent
            // 
            this.tpEvent.Controls.Add(this.dgvEvent);
            this.tpEvent.Location = new System.Drawing.Point(4, 29);
            this.tpEvent.Name = "tpEvent";
            this.tpEvent.Padding = new System.Windows.Forms.Padding(3);
            this.tpEvent.Size = new System.Drawing.Size(964, 506);
            this.tpEvent.TabIndex = 4;
            this.tpEvent.Text = "events";
            this.tpEvent.UseVisualStyleBackColor = true;
            // 
            // dgvEvent
            // 
            this.dgvEvent.AllowUserToOrderColumns = true;
            this.dgvEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEvent.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvEvent.Location = new System.Drawing.Point(3, 3);
            this.dgvEvent.Name = "dgvEvent";
            this.dgvEvent.Size = new System.Drawing.Size(958, 500);
            this.dgvEvent.TabIndex = 2;
            this.dgvEvent.RowContextMenuStripNeeded += new System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventHandler(this.dgvEvent_RowContextMenuStripNeeded);
            this.dgvEvent.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.MasterControl_RowHeaderMouseClick);
            this.dgvEvent.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvEvent_RowPostPaint);
            this.dgvEvent.Leave += new System.EventHandler(this.dgvEvent_Leave);
            // 
            // tpBook
            // 
            this.tpBook.Controls.Add(this.dgvBookedEvent);
            this.tpBook.Location = new System.Drawing.Point(4, 29);
            this.tpBook.Name = "tpBook";
            this.tpBook.Padding = new System.Windows.Forms.Padding(3);
            this.tpBook.Size = new System.Drawing.Size(964, 506);
            this.tpBook.TabIndex = 9;
            this.tpBook.Text = "booked-events";
            this.tpBook.UseVisualStyleBackColor = true;
            // 
            // dgvBookedEvent
            // 
            this.dgvBookedEvent.AllowUserToOrderColumns = true;
            this.dgvBookedEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBookedEvent.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvBookedEvent.Location = new System.Drawing.Point(3, 3);
            this.dgvBookedEvent.Name = "dgvBookedEvent";
            this.dgvBookedEvent.Size = new System.Drawing.Size(958, 500);
            this.dgvBookedEvent.TabIndex = 3;
            this.dgvBookedEvent.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.MasterControl_RowHeaderMouseClick2);
            this.dgvBookedEvent.Leave += new System.EventHandler(this.dgvBookedEvent_Leave);
            // 
            // bnAreas
            // 
            this.bnAreas.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bnAreas.CountItem = this.bindingNavigatorCountItem;
            this.bnAreas.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bnAreas.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bnAreas.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.tsbPrevious,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.nbAreasCurrent,
            this.bindingNavigatorPositionItem,
            this.nbAreasPage,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.tsbNext,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.nbAreasTotal,
            this.tsbGet2});
            this.bnAreas.Location = new System.Drawing.Point(0, 576);
            this.bnAreas.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnAreas.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnAreas.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnAreas.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnAreas.Name = "bnAreas";
            this.bnAreas.PositionItem = this.bindingNavigatorPositionItem;
            this.bnAreas.Size = new System.Drawing.Size(984, 30);
            this.bnAreas.TabIndex = 2;
            this.bnAreas.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.bnAreas_ItemClicked);
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 27);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            this.bindingNavigatorAddNewItem.Visible = false;
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(49, 27);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            this.bindingNavigatorCountItem.Visible = false;
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 27);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            this.bindingNavigatorDeleteItem.Visible = false;
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 27);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // tsbPrevious
            // 
            this.tsbPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPrevious.Image = ((System.Drawing.Image)(resources.GetObject("tsbPrevious.Image")));
            this.tsbPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPrevious.Name = "tsbPrevious";
            this.tsbPrevious.Size = new System.Drawing.Size(23, 27);
            this.tsbPrevious.Text = "Previous";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 27);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            this.bindingNavigatorMovePreviousItem.Visible = false;
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 30);
            // 
            // nbAreasCurrent
            // 
            this.nbAreasCurrent.AutoSize = false;
            this.nbAreasCurrent.Name = "nbAreasCurrent";
            this.nbAreasCurrent.Size = new System.Drawing.Size(50, 30);
            this.nbAreasCurrent.Text = "0";
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 30);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            this.bindingNavigatorPositionItem.Visible = false;
            // 
            // nbAreasPage
            // 
            this.nbAreasPage.Name = "nbAreasPage";
            this.nbAreasPage.Size = new System.Drawing.Size(0, 27);
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // tsbNext
            // 
            this.tsbNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNext.Image = ((System.Drawing.Image)(resources.GetObject("tsbNext.Image")));
            this.tsbNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNext.Name = "tsbNext";
            this.tsbNext.Size = new System.Drawing.Size(23, 27);
            this.tsbNext.Text = "Next";
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 27);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            this.bindingNavigatorMoveNextItem.Visible = false;
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 27);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 30);
            // 
            // nbAreasTotal
            // 
            this.nbAreasTotal.Name = "nbAreasTotal";
            this.nbAreasTotal.Size = new System.Drawing.Size(0, 27);
            // 
            // tsbGet2
            // 
            this.tsbGet2.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tsbGet2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbGet2.Image = ((System.Drawing.Image)(resources.GetObject("tsbGet2.Image")));
            this.tsbGet2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGet2.Name = "tsbGet2";
            this.tsbGet2.Size = new System.Drawing.Size(41, 27);
            this.tsbGet2.Text = "Get";
            this.tsbGet2.Click += new System.EventHandler(this.tsbGet2_Click);
            // 
            // bnAreas2
            // 
            this.bnAreas2.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bnAreas2.CountItem = null;
            this.bnAreas2.DeleteItem = null;
            this.bnAreas2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bnAreas2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslArea,
            this.tsbArea,
            this.tsdAreaParentId,
            this.tsdArea,
            this.tsdComp,
            this.tsdSeason,
            this.tsdStage,
            this.tsdGroup,
            this.tsdPartic,
            this.tsdEvent,
            this.toolStripSeparator1,
            this.tsbGet});
            this.bnAreas2.Location = new System.Drawing.Point(0, 546);
            this.bnAreas2.MoveFirstItem = null;
            this.bnAreas2.MoveLastItem = null;
            this.bnAreas2.MoveNextItem = null;
            this.bnAreas2.MovePreviousItem = null;
            this.bnAreas2.Name = "bnAreas2";
            this.bnAreas2.PositionItem = null;
            this.bnAreas2.Size = new System.Drawing.Size(984, 30);
            this.bnAreas2.TabIndex = 8;
            // 
            // tslArea
            // 
            this.tslArea.Name = "tslArea";
            this.tslArea.Size = new System.Drawing.Size(90, 27);
            this.tslArea.Text = "parent_id :";
            // 
            // tsbArea
            // 
            this.tsbArea.Name = "tsbArea";
            this.tsbArea.Size = new System.Drawing.Size(50, 30);
            this.tsbArea.Text = "All";
            // 
            // tsdAreaParentId
            // 
            this.tsdAreaParentId.AutoSize = false;
            this.tsdAreaParentId.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tsdAreaParentId.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdAreaParentId.Image = ((System.Drawing.Image)(resources.GetObject("tsdAreaParentId.Image")));
            this.tsdAreaParentId.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdAreaParentId.Name = "tsdAreaParentId";
            this.tsdAreaParentId.Size = new System.Drawing.Size(50, 27);
            this.tsdAreaParentId.Text = "id";
            this.tsdAreaParentId.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsdAreaParentId_DropDownItemClicked);
            this.tsdAreaParentId.Click += new System.EventHandler(this.tsdAreaParentId_Click);
            // 
            // tsdArea
            // 
            this.tsdArea.AutoSize = false;
            this.tsdArea.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tsdArea.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tsdArea.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdArea.Image = ((System.Drawing.Image)(resources.GetObject("tsdArea.Image")));
            this.tsdArea.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdArea.Name = "tsdArea";
            this.tsdArea.Size = new System.Drawing.Size(150, 27);
            this.tsdArea.Text = "area";
            this.tsdArea.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsdArea_DropDownItemClicked);
            // 
            // tsdComp
            // 
            this.tsdComp.AutoSize = false;
            this.tsdComp.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tsdComp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdComp.Image = ((System.Drawing.Image)(resources.GetObject("tsdComp.Image")));
            this.tsdComp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdComp.Name = "tsdComp";
            this.tsdComp.Size = new System.Drawing.Size(150, 27);
            this.tsdComp.Text = "competition";
            this.tsdComp.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsdComp_DropDownItemClicked);
            // 
            // tsdSeason
            // 
            this.tsdSeason.AutoSize = false;
            this.tsdSeason.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tsdSeason.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdSeason.Image = ((System.Drawing.Image)(resources.GetObject("tsdSeason.Image")));
            this.tsdSeason.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdSeason.Name = "tsdSeason";
            this.tsdSeason.Size = new System.Drawing.Size(150, 27);
            this.tsdSeason.Text = "season";
            this.tsdSeason.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsdSeason_DropDownItemClicked);
            // 
            // tsdStage
            // 
            this.tsdStage.AutoSize = false;
            this.tsdStage.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tsdStage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdStage.Image = ((System.Drawing.Image)(resources.GetObject("tsdStage.Image")));
            this.tsdStage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdStage.Name = "tsdStage";
            this.tsdStage.Size = new System.Drawing.Size(120, 27);
            this.tsdStage.Text = "stage";
            this.tsdStage.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsdStage_DropDownItemClicked);
            // 
            // tsdGroup
            // 
            this.tsdGroup.AutoSize = false;
            this.tsdGroup.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tsdGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdGroup.Image = ((System.Drawing.Image)(resources.GetObject("tsdGroup.Image")));
            this.tsdGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdGroup.Name = "tsdGroup";
            this.tsdGroup.Size = new System.Drawing.Size(120, 27);
            this.tsdGroup.Text = "group";
            this.tsdGroup.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsdGroup_DropDownItemClicked);
            // 
            // tsdPartic
            // 
            this.tsdPartic.AutoSize = false;
            this.tsdPartic.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tsdPartic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdPartic.Image = ((System.Drawing.Image)(resources.GetObject("tsdPartic.Image")));
            this.tsdPartic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdPartic.Name = "tsdPartic";
            this.tsdPartic.Size = new System.Drawing.Size(150, 27);
            this.tsdPartic.Text = "participant";
            // 
            // tsdEvent
            // 
            this.tsdEvent.AutoSize = false;
            this.tsdEvent.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tsdEvent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdEvent.Image = ((System.Drawing.Image)(resources.GetObject("tsdEvent.Image")));
            this.tsdEvent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdEvent.Name = "tsdEvent";
            this.tsdEvent.Size = new System.Drawing.Size(150, 27);
            this.tsdEvent.Text = "event";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // tsbGet
            // 
            this.tsbGet.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tsbGet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbGet.Image = ((System.Drawing.Image)(resources.GetObject("tsbGet.Image")));
            this.tsbGet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGet.Name = "tsbGet";
            this.tsbGet.Size = new System.Drawing.Size(41, 27);
            this.tsbGet.Text = "Get";
            this.tsbGet.Click += new System.EventHandler(this.tsbGet_Click);
            // 
            // cmsBook
            // 
            this.cmsBook.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miBook});
            this.cmsBook.Name = "cmsBook";
            this.cmsBook.Size = new System.Drawing.Size(119, 32);
            this.cmsBook.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsBook_ItemClicked);
            // 
            // miBook
            // 
            this.miBook.Name = "miBook";
            this.miBook.Size = new System.Drawing.Size(118, 28);
            this.miBook.Text = "Book";
            // 
            // cmsBooked
            // 
            this.cmsBooked.Name = "cmsBooked";
            this.cmsBooked.Size = new System.Drawing.Size(61, 4);
            this.cmsBooked.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsBooked_ItemClicked);
            // 
            // DataOfScouts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 606);
            this.Controls.Add(this.bnAreas2);
            this.Controls.Add(this.bnAreas);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataOfScouts";
            this.Text = Directory.GetCurrentDirectory() + @"\DataOfScouts";

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataOfScouts_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DataOfScouts_FormClosed);
            this.Load += new System.EventHandler(this.DataOfScouts_Load);
            this.tabControl1.ResumeLayout(false);
            this.tpAuthorization.ResumeLayout(false);
            this.tpAuthorization.PerformLayout();
            this.gbAuthorization.ResumeLayout(false);
            this.tpAreas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAreas)).EndInit();
            this.tpCompetitions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvComp)).EndInit();
            this.tpSeasons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeasons)).EndInit();
            this.tpStages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStages)).EndInit();
            this.tpGroups.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).EndInit();
            this.tpTeam.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTeam)).EndInit();
            this.tpPlayer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayer)).EndInit();
            this.tpEvent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEvent)).EndInit();
            this.tpBook.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBookedEvent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnAreas)).EndInit();
            this.bnAreas.ResumeLayout(false);
            this.bnAreas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnAreas2)).EndInit();
            this.bnAreas2.ResumeLayout(false);
            this.bnAreas2.PerformLayout();
            this.cmsBook.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
 
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpAreas;
        private System.Windows.Forms.TabPage tpCompetitions;
        private System.Windows.Forms.TabPage tpSeasons;
        private System.Windows.Forms.TabPage tpStages;
        private System.Windows.Forms.TabPage tpGroups;
        private System.Windows.Forms.TabPage tpEvent;
        private System.Windows.Forms.DataGridView dgvAreas;
        private System.Windows.Forms.TabPage tpAuthorization;
        private System.Windows.Forms.GroupBox gbAuthorization;
        private System.Windows.Forms.Label lbAuthorization;
        private System.Windows.Forms.Label lbToken;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingNavigator bnAreas;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton tsbPrevious;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox nbAreasCurrent;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripLabel nbAreasPage;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton tsbNext;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripLabel nbAreasTotal;
        private System.Windows.Forms.DataGridView dgvComp;
        private System.Windows.Forms.DataGridView dgvSeasons;
        //private System.Windows.Forms.DataGridView dgvEvent;
        private MasterDetailSample.MasterControl  dgvEvent;
        private System.Windows.Forms.BindingNavigator bnAreas2;
        private System.Windows.Forms.ToolStripLabel tslArea;
        private System.Windows.Forms.ToolStripTextBox tsbArea;
        private System.Windows.Forms.ToolStripDropDownButton tsdArea;
        private System.Windows.Forms.ToolStripDropDownButton tsdComp;
        private System.Windows.Forms.ToolStripDropDownButton tsdSeason;
        private System.Windows.Forms.ToolStripDropDownButton tsdStage;
        private System.Windows.Forms.ToolStripDropDownButton tsdGroup;
        private System.Windows.Forms.ToolStripDropDownButton tsdPartic;
        private System.Windows.Forms.ToolStripDropDownButton tsdEvent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbGet;
        private System.Windows.Forms.ToolStripDropDownButton tsdAreaParentId;
        private System.Windows.Forms.DataGridView dgvStages;
        private System.Windows.Forms.DataGridView dgvGroups;
        private System.Windows.Forms.TabPage tpTeam;
        private System.Windows.Forms.DataGridView dgvTeam;
        private System.Windows.Forms.TabPage tpPlayer;
        private System.Windows.Forms.DataGridView dgvPlayer;
        private System.Windows.Forms.Label lbResults;
        private System.Windows.Forms.TabPage tpBook;
        //private System.Windows.Forms.DataGridView dgvBookedEvent;
        private MasterDetailSample.MasterControl dgvBookedEvent;
        private System.Windows.Forms.ToolStripButton tsbGet2;
        private System.Windows.Forms.ContextMenuStrip cmsBook;
        private System.Windows.Forms.ContextMenuStrip cmsBooked;
        private System.Windows.Forms.ToolStripMenuItem miBook;
        private System.Windows.Forms.ListBox lstStatus;
        private System.Windows.Forms.TextBox txtXmlFileUrl;
        private System.Windows.Forms.Button btnXmlParser;
        private System.Windows.Forms.Button btnSelectFile;
    }
}

