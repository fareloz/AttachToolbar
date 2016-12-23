using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AttachToolbar
{
    public class OptionsGeneralWindow : UserControl
    {
        public void Initialize()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            buttonAdd = new Button();
            textboxProcessName = new TextBox();
            listviewProcessList = new ListView();
            labelProcessName = new Label();
            buttonRemove = new Button();
            buttonClear = new Button();
            columnProcessName = new ColumnHeader();
            SuspendLayout();
            // 
            // buttonAdd
            // 
            buttonAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            buttonAdd.Location = new System.Drawing.Point(361, 10);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new System.Drawing.Size(108, 31);
            buttonAdd.TabIndex = 1;
            buttonAdd.Text = "Add";
            buttonAdd.UseVisualStyleBackColor = true;
            buttonAdd.Click += buttonAdd_Click;
            // 
            // textboxProcessName
            // 
            textboxProcessName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            textboxProcessName.Location = new System.Drawing.Point(146, 12);
            textboxProcessName.Name = "textboxProcessName";
            textboxProcessName.Size = new System.Drawing.Size(203, 26);
            textboxProcessName.TabIndex = 0;
            // 
            // listviewProcessList
            // 
            listviewProcessList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnProcessName});
            listviewProcessList.GridLines = true;
            listviewProcessList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listviewProcessList.Location = new System.Drawing.Point(20, 51);
            listviewProcessList.Name = "listviewProcessList";
            listviewProcessList.Size = new System.Drawing.Size(329, 202);
            listviewProcessList.TabIndex = 4;
            listviewProcessList.UseCompatibleStateImageBehavior = false;
            listviewProcessList.View = View.Details;
            // 
            // labelProcessName
            // 
            labelProcessName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            labelProcessName.Location = new System.Drawing.Point(17, 15);
            labelProcessName.Name = "labelProcessName";
            labelProcessName.Size = new System.Drawing.Size(128, 28);
            labelProcessName.TabIndex = 5;
            labelProcessName.Text = "Process name:";
            // 
            // buttonRemove
            // 
            buttonRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            buttonRemove.Location = new System.Drawing.Point(361, 51);
            buttonRemove.Name = "buttonRemove";
            buttonRemove.Size = new System.Drawing.Size(108, 31);
            buttonRemove.TabIndex = 2;
            buttonRemove.Text = "Remove";
            buttonRemove.UseVisualStyleBackColor = true;
            buttonRemove.Click += buttonRemove_Click;
            // 
            // buttonClear
            // 
            buttonClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            buttonClear.Location = new System.Drawing.Point(361, 88);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new System.Drawing.Size(108, 31);
            buttonClear.TabIndex = 3;
            buttonClear.Text = "Clear";
            buttonClear.UseVisualStyleBackColor = true;
            buttonClear.Click += buttonClear_Click;
            // 
            // columnProcessName
            // 
            columnProcessName.Text = "Process name";
            columnProcessName.Width = 325;
            // 
            // OptionsGeneralWindow
            // 
            Controls.Add(buttonClear);
            Controls.Add(buttonRemove);
            Controls.Add(labelProcessName);
            Controls.Add(listviewProcessList);
            Controls.Add(textboxProcessName);
            Controls.Add(buttonAdd);
            Name = "OptionsGeneralWindow";
            Size = new System.Drawing.Size(499, 277);
            ResumeLayout(false);
            PerformLayout();
        }

        private void buttonAdd_Click(object sender, System.EventArgs e)
        {
            string processName = textboxProcessName.Text;
            if(processName == string.Empty)
                return;
            listviewProcessList.Items.Add(processName);
        }

        private void buttonRemove_Click(object sender, System.EventArgs e)
        {
            while (listviewProcessList.SelectedItems.Count != 0)
                listviewProcessList.SelectedItems[0].Remove();
        }

        private void buttonClear_Click(object sender, System.EventArgs e)
        {
            listviewProcessList.Items.Clear();
        }

        private Button buttonAdd;
        private TextBox textboxProcessName;

        private ListView listviewProcessList;
        public ColumnHeader columnProcessName;

        private Label labelProcessName;
        private Button buttonRemove;
        private Button buttonClear;

        private readonly OptionsPage _page;
    }
}
