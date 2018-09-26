﻿using System.IO;

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
            this.gbAuthorization = new System.Windows.Forms.GroupBox();
            this.lbToken = new System.Windows.Forms.Label();
            this.lbAuthorization = new System.Windows.Forms.Label();
            this.tpAreas = new System.Windows.Forms.TabPage();
            this.dgvAreas = new System.Windows.Forms.DataGridView();
            this.tpCompetitions = new System.Windows.Forms.TabPage();
            this.dgvComp = new System.Windows.Forms.DataGridView();
            this.tpSeasons = new System.Windows.Forms.TabPage();
            this.dgvSeasons = new System.Windows.Forms.DataGridView();
            this.tpStages = new System.Windows.Forms.TabPage();
            this.tpGroups = new System.Windows.Forms.TabPage();
            this.tpParticipants = new System.Windows.Forms.TabPage();
            this.dgvPart = new System.Windows.Forms.DataGridView();
            this.tpEvent = new System.Windows.Forms.TabPage();
            this.dgvEvent = new System.Windows.Forms.DataGridView();
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
            this.bnAreas2 = new System.Windows.Forms.BindingNavigator(this.components);
            this.tslArea = new System.Windows.Forms.ToolStripLabel();
            this.tsbArea = new System.Windows.Forms.ToolStripTextBox();
            this.tsdArea = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsdComp = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsdSeason = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsdStage = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsdGroup = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsdPartic = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsdEvent = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbGet = new System.Windows.Forms.ToolStripButton();
            this.tabControl1.SuspendLayout();
            this.tpAuthorization.SuspendLayout();
            this.gbAuthorization.SuspendLayout();
            this.tpAreas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAreas)).BeginInit();
            this.tpCompetitions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComp)).BeginInit();
            this.tpSeasons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeasons)).BeginInit();
            this.tpParticipants.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPart)).BeginInit();
            this.tpEvent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEvent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnAreas)).BeginInit();
            this.bnAreas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnAreas2)).BeginInit();
            this.bnAreas2.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.tpParticipants);
            this.tabControl1.Controls.Add(this.tpEvent);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tabControl1.Location = new System.Drawing.Point(0, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(853, 539);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tpAuthorization
            // 
            this.tpAuthorization.Controls.Add(this.gbAuthorization);
            this.tpAuthorization.Location = new System.Drawing.Point(4, 29);
            this.tpAuthorization.Name = "tpAuthorization";
            this.tpAuthorization.Size = new System.Drawing.Size(845, 506);
            this.tpAuthorization.TabIndex = 5;
            this.tpAuthorization.Text = "Authorization";
            this.tpAuthorization.UseVisualStyleBackColor = true;
            // 
            // gbAuthorization
            // 
            this.gbAuthorization.Controls.Add(this.lbToken);
            this.gbAuthorization.Controls.Add(this.lbAuthorization);
            this.gbAuthorization.Location = new System.Drawing.Point(3, 17);
            this.gbAuthorization.Name = "gbAuthorization";
            this.gbAuthorization.Size = new System.Drawing.Size(505, 139);
            this.gbAuthorization.TabIndex = 0;
            this.gbAuthorization.TabStop = false;
            // 
            // lbToken
            // 
            this.lbToken.AutoSize = true;
            this.lbToken.Location = new System.Drawing.Point(26, 82);
            this.lbToken.Name = "lbToken";
            this.lbToken.Size = new System.Drawing.Size(0, 20);
            this.lbToken.TabIndex = 1;
            // 
            // lbAuthorization
            // 
            this.lbAuthorization.AutoSize = true;
            this.lbAuthorization.Location = new System.Drawing.Point(22, 32);
            this.lbAuthorization.Name = "lbAuthorization";
            this.lbAuthorization.Size = new System.Drawing.Size(0, 20);
            this.lbAuthorization.TabIndex = 0;
            // 
            // tpAreas
            // 
            this.tpAreas.Controls.Add(this.dgvAreas);
            this.tpAreas.Location = new System.Drawing.Point(4, 29);
            this.tpAreas.Name = "tpAreas";
            this.tpAreas.Size = new System.Drawing.Size(845, 506);
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
            this.dgvAreas.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.dgvAreas.Size = new System.Drawing.Size(839, 490);
            this.dgvAreas.TabIndex = 0;
            // 
            // tpCompetitions
            // 
            this.tpCompetitions.Controls.Add(this.dgvComp);
            this.tpCompetitions.Location = new System.Drawing.Point(4, 29);
            this.tpCompetitions.Name = "tpCompetitions";
            this.tpCompetitions.Size = new System.Drawing.Size(845, 506);
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
            this.dgvComp.Size = new System.Drawing.Size(839, 490);
            this.dgvComp.TabIndex = 1;
            // 
            // tpSeasons
            // 
            this.tpSeasons.Controls.Add(this.dgvSeasons);
            this.tpSeasons.Location = new System.Drawing.Point(4, 29);
            this.tpSeasons.Name = "tpSeasons";
            this.tpSeasons.Padding = new System.Windows.Forms.Padding(3);
            this.tpSeasons.Size = new System.Drawing.Size(845, 506);
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
            this.dgvSeasons.Size = new System.Drawing.Size(839, 490);
            this.dgvSeasons.TabIndex = 1;
            // 
            // tpStages
            // 
            this.tpStages.Location = new System.Drawing.Point(4, 29);
            this.tpStages.Name = "tpStages";
            this.tpStages.Padding = new System.Windows.Forms.Padding(3);
            this.tpStages.Size = new System.Drawing.Size(845, 506);
            this.tpStages.TabIndex = 2;
            this.tpStages.Text = "stages";
            this.tpStages.UseVisualStyleBackColor = true;
            // 
            // tpGroups
            // 
            this.tpGroups.Location = new System.Drawing.Point(4, 29);
            this.tpGroups.Name = "tpGroups";
            this.tpGroups.Padding = new System.Windows.Forms.Padding(3);
            this.tpGroups.Size = new System.Drawing.Size(845, 506);
            this.tpGroups.TabIndex = 3;
            this.tpGroups.Text = "groups";
            this.tpGroups.UseVisualStyleBackColor = true;
            // 
            // tpParticipants
            // 
            this.tpParticipants.Controls.Add(this.dgvPart);
            this.tpParticipants.Location = new System.Drawing.Point(4, 29);
            this.tpParticipants.Name = "tpParticipants";
            this.tpParticipants.Size = new System.Drawing.Size(845, 506);
            this.tpParticipants.TabIndex = 6;
            this.tpParticipants.Text = "participants";
            this.tpParticipants.UseVisualStyleBackColor = true;
            // 
            // dgvPart
            // 
            this.dgvPart.AllowUserToOrderColumns = true;
            this.dgvPart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPart.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvPart.Location = new System.Drawing.Point(3, 3);
            this.dgvPart.Name = "dgvPart";
            this.dgvPart.Size = new System.Drawing.Size(839, 490);
            this.dgvPart.TabIndex = 3;
            // 
            // tpEvent
            // 
            this.tpEvent.Controls.Add(this.dgvEvent);
            this.tpEvent.Location = new System.Drawing.Point(4, 29);
            this.tpEvent.Name = "tpEvent";
            this.tpEvent.Padding = new System.Windows.Forms.Padding(3);
            this.tpEvent.Size = new System.Drawing.Size(845, 506);
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
            this.dgvEvent.Size = new System.Drawing.Size(839, 490);
            this.dgvEvent.TabIndex = 2;
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
            this.nbAreasTotal});
            this.bnAreas.Location = new System.Drawing.Point(0, 576);
            this.bnAreas.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnAreas.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnAreas.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnAreas.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnAreas.Name = "bnAreas";
            this.bnAreas.PositionItem = this.bindingNavigatorPositionItem;
            this.bnAreas.Size = new System.Drawing.Size(865, 30);
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
            // bnAreas2
            // 
            this.bnAreas2.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bnAreas2.CountItem = null;
            this.bnAreas2.DeleteItem = null;
            this.bnAreas2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bnAreas2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslArea,
            this.tsbArea,
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
            this.bnAreas2.Size = new System.Drawing.Size(865, 30);
            this.bnAreas2.TabIndex = 8;
            // 
            // tslArea
            // 
            this.tslArea.Name = "tslArea";
            this.tslArea.Size = new System.Drawing.Size(52, 27);
            this.tslArea.Text = "area :";
            // 
            // tsbArea
            // 
            this.tsbArea.Name = "tsbArea";
            this.tsbArea.Size = new System.Drawing.Size(50, 30);
            // 
            // tsdArea
            // 
            this.tsdArea.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tsdArea.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tsdArea.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdArea.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.tsdArea.Image = ((System.Drawing.Image)(resources.GetObject("tsdArea.Image")));
            this.tsdArea.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdArea.Name = "tsdArea";
            this.tsdArea.Size = new System.Drawing.Size(56, 27);
            this.tsdArea.Text = "area";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(224, 28);
            this.toolStripMenuItem1.Text = "aaaaaaaaaaaaaaaa";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(224, 28);
            this.toolStripMenuItem2.Text = "bbbbbbbbbbbbb";
            // 
            // tsdComp
            // 
            this.tsdComp.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tsdComp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdComp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.tsdComp.Image = ((System.Drawing.Image)(resources.GetObject("tsdComp.Image")));
            this.tsdComp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdComp.Name = "tsdComp";
            this.tsdComp.Size = new System.Drawing.Size(115, 27);
            this.tsdComp.Text = "competition";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(216, 28);
            this.toolStripMenuItem3.Text = "xxxxxxxxxxxxx";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(216, 28);
            this.toolStripMenuItem4.Text = "ccccccccccccccccc";
            // 
            // tsdSeason
            // 
            this.tsdSeason.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tsdSeason.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdSeason.Image = ((System.Drawing.Image)(resources.GetObject("tsdSeason.Image")));
            this.tsdSeason.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdSeason.Name = "tsdSeason";
            this.tsdSeason.Size = new System.Drawing.Size(75, 27);
            this.tsdSeason.Text = "season";
            // 
            // tsdStage
            // 
            this.tsdStage.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tsdStage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdStage.Image = ((System.Drawing.Image)(resources.GetObject("tsdStage.Image")));
            this.tsdStage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdStage.Name = "tsdStage";
            this.tsdStage.Size = new System.Drawing.Size(66, 27);
            this.tsdStage.Text = "Stage";
            // 
            // tsdGroup
            // 
            this.tsdGroup.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tsdGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdGroup.Image = ((System.Drawing.Image)(resources.GetObject("tsdGroup.Image")));
            this.tsdGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdGroup.Name = "tsdGroup";
            this.tsdGroup.Size = new System.Drawing.Size(69, 27);
            this.tsdGroup.Text = "group";
            // 
            // tsdPartic
            // 
            this.tsdPartic.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tsdPartic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdPartic.Image = ((System.Drawing.Image)(resources.GetObject("tsdPartic.Image")));
            this.tsdPartic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdPartic.Name = "tsdPartic";
            this.tsdPartic.Size = new System.Drawing.Size(105, 27);
            this.tsdPartic.Text = "participant";
            // 
            // tsdEvent
            // 
            this.tsdEvent.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tsdEvent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsdEvent.Image = ((System.Drawing.Image)(resources.GetObject("tsdEvent.Image")));
            this.tsdEvent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsdEvent.Name = "tsdEvent";
            this.tsdEvent.Size = new System.Drawing.Size(65, 27);
            this.tsdEvent.Text = "event";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // tsbGet
            // 
            this.tsbGet.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tsbGet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbGet.Image = ((System.Drawing.Image)(resources.GetObject("tsbGet.Image")));
            this.tsbGet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGet.Name = "tsbGet";
            this.tsbGet.Size = new System.Drawing.Size(41, 27);
            this.tsbGet.Text = "Get";
            this.tsbGet.Click += new System.EventHandler(this.tsbGet_Click);
            // 
            // DataOfScouts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(865, 606);
            this.Controls.Add(this.bnAreas2);
            this.Controls.Add(this.bnAreas);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataOfScouts";
            this.Text = "D:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Professional\\Common7\\IDE\\Data" +
    "OfScouts";
            this.tabControl1.ResumeLayout(false);
            this.tpAuthorization.ResumeLayout(false);
            this.gbAuthorization.ResumeLayout(false);
            this.gbAuthorization.PerformLayout();
            this.tpAreas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAreas)).EndInit();
            this.tpCompetitions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvComp)).EndInit();
            this.tpSeasons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeasons)).EndInit();
            this.tpParticipants.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPart)).EndInit();
            this.tpEvent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEvent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnAreas)).EndInit();
            this.bnAreas.ResumeLayout(false);
            this.bnAreas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnAreas2)).EndInit();
            this.bnAreas2.ResumeLayout(false);
            this.bnAreas2.PerformLayout();
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
        private System.Windows.Forms.DataGridView dgvEvent;
        private System.Windows.Forms.TabPage tpParticipants;
        private System.Windows.Forms.DataGridView dgvPart;
        private System.Windows.Forms.BindingNavigator bnAreas2;
        private System.Windows.Forms.ToolStripLabel tslArea;
        private System.Windows.Forms.ToolStripTextBox tsbArea;
        private System.Windows.Forms.ToolStripDropDownButton tsdArea;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripDropDownButton tsdComp;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripDropDownButton tsdSeason;
        private System.Windows.Forms.ToolStripDropDownButton tsdStage;
        private System.Windows.Forms.ToolStripDropDownButton tsdGroup;
        private System.Windows.Forms.ToolStripDropDownButton tsdPartic;
        private System.Windows.Forms.ToolStripDropDownButton tsdEvent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbGet;
    }
}

