using System.Windows.Forms;

namespace AttachToolbar
{
    public class OptionsGeneralWindow : UserControl
    {
        public void Initialize()
        {
            InitializeComponent();
            foreach (var processName in State.ProcessList)
                listviewProcessList.Items.Add(processName);
        }

        private void InitializeComponent()
        {
            this.buttonAdd = new System.Windows.Forms.Button();
            this.textboxProcessName = new System.Windows.Forms.TextBox();
            this.listviewProcessList = new System.Windows.Forms.ListView();
            this.columnProcessName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelProcessName = new System.Windows.Forms.Label();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonAdd
            // 
            this.buttonAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAdd.Location = new System.Drawing.Point(361, 10);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(108, 31);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // textboxProcessName
            // 
            this.textboxProcessName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textboxProcessName.Location = new System.Drawing.Point(146, 12);
            this.textboxProcessName.Name = "textboxProcessName";
            this.textboxProcessName.Size = new System.Drawing.Size(203, 26);
            this.textboxProcessName.TabIndex = 0;
            // 
            // listviewProcessList
            // 
            this.listviewProcessList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnProcessName});
            this.listviewProcessList.FullRowSelect = true;
            this.listviewProcessList.GridLines = true;
            this.listviewProcessList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listviewProcessList.HideSelection = false;
            this.listviewProcessList.Location = new System.Drawing.Point(20, 51);
            this.listviewProcessList.Name = "listviewProcessList";
            this.listviewProcessList.Size = new System.Drawing.Size(329, 202);
            this.listviewProcessList.TabIndex = 4;
            this.listviewProcessList.UseCompatibleStateImageBehavior = false;
            this.listviewProcessList.View = System.Windows.Forms.View.Details;
            // 
            // columnProcessName
            // 
            this.columnProcessName.Text = "Process name";
            this.columnProcessName.Width = 325;
            // 
            // labelProcessName
            // 
            this.labelProcessName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelProcessName.Location = new System.Drawing.Point(17, 15);
            this.labelProcessName.Name = "labelProcessName";
            this.labelProcessName.Size = new System.Drawing.Size(128, 28);
            this.labelProcessName.TabIndex = 5;
            this.labelProcessName.Text = "Process name:";
            // 
            // buttonRemove
            // 
            this.buttonRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonRemove.Location = new System.Drawing.Point(361, 51);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(108, 31);
            this.buttonRemove.TabIndex = 2;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonClear.Location = new System.Drawing.Point(361, 88);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(108, 31);
            this.buttonClear.TabIndex = 3;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // OptionsGeneralWindow
            // 
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.labelProcessName);
            this.Controls.Add(this.listviewProcessList);
            this.Controls.Add(this.textboxProcessName);
            this.Controls.Add(this.buttonAdd);
            this.Name = "OptionsGeneralWindow";
            this.Size = new System.Drawing.Size(499, 277);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void buttonAdd_Click(object sender, System.EventArgs e)
        {
            string processName = textboxProcessName.Text;
            if(string.IsNullOrEmpty(processName))
                return;

            textboxProcessName.Clear();
            if (listviewProcessList.FindItemWithText(processName) != null)
                return;

            listviewProcessList.Items.Add(processName);
            State.ProcessList.Add(processName);

            if (State.ProcessIndex == -1)
                State.ProcessIndex = 0;
        }

        private void buttonRemove_Click(object sender, System.EventArgs e)
        {
            if (listviewProcessList.SelectedIndices.Count == 0)
                return;

            while (State.ProcessIndex < listviewProcessList.Items.Count &&
                listviewProcessList.SelectedIndices.Contains(State.ProcessIndex))
            {
                ++State.ProcessIndex;
                if (State.ProcessIndex >= listviewProcessList.SelectedIndices.Count)
                {
                    State.ProcessIndex = 0;
                    break;
                }
            }

            while (listviewProcessList.SelectedItems.Count != 0)
                listviewProcessList.SelectedItems[0].Remove();

            if (listviewProcessList.Items.Count == 0)
                State.ProcessIndex = -1;

            State.ProcessList.Clear();
            foreach (ListViewItem item in listviewProcessList.Items)
            {
                State.ProcessList.Add(item.Text);
            }
        }

        private void buttonClear_Click(object sender, System.EventArgs e)
        {
            listviewProcessList.Items.Clear();
            State.ProcessIndex = -1;
            State.ProcessList.Clear();
        }

        private Button buttonAdd;
        private TextBox textboxProcessName;

        private ListView listviewProcessList;
        private ColumnHeader columnProcessName;

        private Label labelProcessName;
        private Button buttonRemove;
        private Button buttonClear;

    }
}
