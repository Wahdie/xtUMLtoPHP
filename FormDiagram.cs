using DocumentFormat.OpenXml.Spreadsheet;
using MindFusion.Diagramming.Layout;
using MindFusion.Diagramming.WinForms;
using MindFusion.Diagramming;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System;
using MindFusion.Drawing;
using static xtUML1.Form1;
using System.Drawing;
using static xtUML1.Visualize;
using MindFusion.Graphs;
using Rect = MindFusion.Graphs.Rectangle;
using Pen = System.Drawing.Pen;
using Brush = System.Drawing.Brush;
using SolidBrush = System.Drawing.SolidBrush;
using Font = System.Drawing.Font;
using Color = System.Drawing.Color;
using static xtUML1.Translate.JsonData;
using LinkLabel = MindFusion.Diagramming.LinkLabel;

namespace xtUML1
{

    public partial class FormDiagram : Form
    {
        private Visualize visualizer;


        public FormDiagram()
        {
            InitializeDiagramView();
            visualizer = new Visualize();

        }

        private void InitializeDiagramView()
        {
            diagramView = new DiagramView { Diagram = new Diagram(), Dock = DockStyle.Fill };
            Controls.Add(diagramView);
        }


        public void ShowDiagram(List<ClassModel> classList, List<AssociationModel> associationList)
        {
            if (diagramView != null)
            {
                diagramView.Diagram.ClearAll();
                CreateDiagram(classList, associationList);
                Show();
            }
        }

        private void CreateDiagram(List<ClassModel> classList, List<AssociationModel> associationList)
        {
            if (diagramView != null && diagramView.Diagram != null)
            {
                var diagram = diagramView.Diagram;
                diagram.ClearAll(); 

                var nodes = new Dictionary<string, DiagramNode>();
                var processedAssociations = new HashSet<string>();

                // Process classes
                foreach (var cls in classList)
                {
                    int x = (cls.ClassId.GetHashCode() % 2 == 0) ? 100 : 300;
                    int y = (cls.ClassId.GetHashCode() / 2) * 50 + 10;

                    var currentNode = diagram.Factory.CreateTableNode(0, 0, 80, 60, 2, 2);
                    currentNode.Caption = $"+{cls.ClassName}";

                    currentNode.CellFrameStyle = CellFrameStyle.Simple;
                    currentNode.Brush = new MindFusion.Drawing.SolidBrush(Color.FromArgb(214, 213, 142));
                    currentNode.CaptionBackBrush = new MindFusion.Drawing.SolidBrush(Color.FromArgb(159, 193, 49));


                    var associationName = associationList
                        .Where(assoc => assoc.Classes.Any(c => c.ClassId == cls.ClassId))
                        .Select(assoc => assoc.Name)
                        .FirstOrDefault();

                    if (cls.Attributes.Any())
                    {
                        foreach (var attr in cls.Attributes)
                        {
                            currentNode.AddRow();
                            int r = currentNode.RowCount - 1;

                            if (attr.AttributeType == "referential_attribute")
                            {
                                currentNode[0, r].Text = attr.AttributeName;
                                currentNode[1, r].Text = attr.DataType + $" ({associationName})";
                            }
                            else
                            {
                                currentNode[0, r].Text = attr.AttributeName;
                                currentNode[1, r].Text = attr.DataType;
                            }
                        }
                    }
                    currentNode.ResizeToFitText(false, false);
                    currentNode.Caption = cls.ClassName;
                    currentNode.ConnectionStyle = TableConnectionStyle.Table;
                    nodes[cls.ClassId] = currentNode;
                }

                // Process associations
                foreach (var assoc in associationList)
                {
                    if (assoc.AssociationClass != null)
                    {
                        var assocClass = assoc.AssociationClass;
                        int x = (assocClass.ClassId.GetHashCode() % 2 == 0) ? 200 : 400;
                        int y = (assocClass.ClassId.GetHashCode() / 2) * 50 + 30;

                        var assocClassNode = diagram.Factory.CreateTableNode(0, 0, 80, 60, 2, 2);
                        assocClassNode.Caption = $"+{assocClass.ClassName}";

                        assocClassNode.CellFrameStyle = CellFrameStyle.Simple;
                        assocClassNode.Brush = new MindFusion.Drawing.SolidBrush(Color.FromArgb(214, 213, 142));
                        assocClassNode.CaptionBackBrush = new MindFusion.Drawing.SolidBrush(Color.FromArgb(159, 193, 49));


                        

                        if (assocClass.Attributes.Any())
                        {
                            foreach (var attr in assocClass.Attributes)
                            {
                                assocClassNode.AddRow();
                                int r = assocClassNode.RowCount - 1;

                                if (attr.AttributeType == "referential_attribute")
                                {
                                    assocClassNode[0, r].Text = attr.AttributeName;
                                    assocClassNode[1, r].Text = attr.DataType + $" ({assoc.Name})";
                                }
                                else
                                {
                                    assocClassNode[0, r].Text = attr.AttributeName;
                                    assocClassNode[1, r].Text = attr.DataType;
                                }
                            }
                            

                        }
                        assocClassNode.ResizeToFitText(false, false);
                        assocClassNode.Caption = assocClass.ClassName;
                        assocClassNode.ConnectionStyle = TableConnectionStyle.Table;
                        nodes[assocClass.ClassId] = assocClassNode;

                        foreach (var cls in assoc.Classes)
                        {
                            if (nodes != null && nodes.TryGetValue(cls.ClassId, out var fromNode))
                            {
                                var toNode = assocClassNode;
                                if (diagram?.Factory != null)
                                {
                                    var link = diagram.Factory.CreateDiagramLink(fromNode, toNode);
                                    link.Text = assoc.Name;
                                    link.HeadShapeSize = 0;
                                    link.BaseShapeSize = 0;

                                    var labelText = $"({cls.Multiplicity}) \n {cls.RoleName}";
                                    var linkLabel = new LinkLabel(link, labelText);
                                    linkLabel.RelativeTo = RelativeToLink.LinkLength;
                                    linkLabel.LengthFactor = 1;
                                    linkLabel.SetLinkLengthPosition(0.29f);
                                    link.AddLabel(linkLabel);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Handle direct associations without association class
                        for (int i = 0; i < assoc.Classes.Count - 1; i++)
                        {
                            for (int j = i + 1; j < assoc.Classes.Count; j++)
                            {
                                var cls1 = assoc.Classes[i];
                                var cls2 = assoc.Classes[j];

                                var linkKey = $"{cls1.ClassId}-{cls2.ClassId}";
                                if (!processedAssociations.Contains(linkKey))
                                {
                                    processedAssociations.Add(linkKey);

                                    if (nodes.ContainsKey(cls1.ClassId) && nodes.ContainsKey(cls2.ClassId))
                                    {
                                        var fromNode = nodes[cls1.ClassId];
                                        var toNode = nodes[cls2.ClassId];

                                        var link = diagram.Factory.CreateDiagramLink(fromNode, toNode);
                                        link.Text = assoc.Name;
                                        link.HeadShapeSize = 0;
                                        link.BaseShapeSize = 0;

                                        var labelText1 = $"({cls1.Multiplicity}) \n {cls1.RoleName}";
                                        var linkLabel1 = new LinkLabel(link, labelText1);
                                        linkLabel1.RelativeTo = RelativeToLink.LinkLength;
                                        linkLabel1.LengthFactor = 1;
                                        linkLabel1.SetLinkLengthPosition(0.29f);

                                        var labelText2 = $" {cls2.RoleName} \n({cls2.Multiplicity}) ";
                                        var linkLabel2 = new LinkLabel(link, labelText2);
                                        linkLabel2.RelativeTo = RelativeToLink.LinkLength;
                                        linkLabel2.LengthFactor = 1;
                                        linkLabel2.SetLinkLengthPosition(0.99f);
                                        link.AddLabel(linkLabel1);
                                        link.AddLabel(linkLabel2);
                                        link.AddLabel(linkLabel1);

                                    }
                                }
                            }
                        }
                    }
                }

                // Arrange the diagram
                var layout = new LayeredLayout
                {
                    EnforceLinkFlow = true,
                    IgnoreNodeSize = false,
                    NodeDistance = 50,
                    LayerDistance = 40
                };
                layout.Arrange(diagram);
                diagram.ResizeToFitItems(5);
            }
        }
    }
}






