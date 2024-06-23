namespace xtUML1
{
    partial class FormDiagram
    {
        private MindFusion.Diagramming.WinForms.DiagramView diagramView;
        private System.ComponentModel.IContainer components = null;
        private MindFusion.Diagramming.Diagram diagram;
        private System.Windows.Forms.TextBox textBox1;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        //private System.ComponentModel.IContainer components = null;

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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.diagram = new MindFusion.Diagramming.Diagram();
            this.diagramView = new MindFusion.Diagramming.WinForms.DiagramView();
            this.textBox1 = new System.Windows.Forms.TextBox();

            // Initialize diagram
            this.diagramView.Diagram = this.diagram;
            this.diagramView.Dock = System.Windows.Forms.DockStyle.Top;
            this.diagramView.Location = new System.Drawing.Point(0, 0);
            this.diagramView.Name = "diagramView";
            this.diagramView.Size = new System.Drawing.Size(800, 400);
            this.diagramView.TabIndex = 0;

            // Initialize textBox1
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 400);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(800, 100);
            this.textBox1.TabIndex = 1;

            // Add controls to the form
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.diagramView);

            // Set form properties
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Name = "FormDiagram";
            this.Text = "Class Diagram";
        }
    }
}
