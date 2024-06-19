using DocumentFormat.OpenXml.Spreadsheet;
using MindFusion.Diagramming.Layout;
using MindFusion.Diagramming.WinForms;
using MindFusion.Diagramming;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System;
using MindFusion.Diagramming.Components;
using static xtUML1.Form1;
using System.Drawing;
using static xtUML1.Visualize;

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
                diagram.ClearAll(); // Clear existing nodes and links

                var nodes = new Dictionary<string, ShapeNode>();
                var processedAssociations = new HashSet<string>(); // To avoid duplicate associations

                // Process classes
                foreach (var cls in classList)
                {
                    int x = (cls.ClassId.GetHashCode() % 2 == 0) ? 100 : 300;
                    int y = (cls.ClassId.GetHashCode() / 2) * 50 + 10;

                    var currentNode = diagram.Factory.CreateShapeNode(x, y, 60, 50); // Smaller node size
                    currentNode.Text = $"Class ID: {cls.ClassId}\n+{cls.ClassName}\n({cls.KL})";
                    var associationName = associationList
                        .Where(assoc => assoc.Classes.Any(c => c.ClassId == cls.ClassId))
                        .Select(assoc => assoc.Name)
                        .FirstOrDefault();

                    if (cls.Attributes.Any())
                    {
                        currentNode.Text += "\n---------------------------------------------------";
                        foreach (var attr in cls.Attributes)
                        {
                            if (attr.AttributeType == "referential_attribute")
                            {
                                currentNode.Text += $"\n {attr.AttributeName} : {attr.DataType}   ({associationName})";
                            }
                            else
                            {
                                currentNode.Text += $"\n {attr.AttributeName} : {attr.DataType}";
                            }
                        }
                    }

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

                        var assocClassNode = diagram.Factory.CreateShapeNode(x, y, 60, 50); 
                        assocClassNode.Text = $"Assoc Class ID: {assocClass.ClassId}\n+{assocClass.ClassName}\n({assocClass.KL})";

                        if (assocClass.Attributes.Any())
                        {
                            assocClassNode.Text += "\n---------------------------------------------------";
                            foreach (var attr in assocClass.Attributes)
                            {
                                if (attr.AttributeType == "referential_attribute")
                                {
                                    assocClassNode.Text += $"\n {attr.AttributeName} : {attr.DataType}   ({assoc.Name})";
                                }
                                else
                                {
                                    assocClassNode.Text += $"\n {attr.AttributeName} : {attr.DataType}";

                                }
                            }
                        }

                        nodes[assocClass.ClassId] = assocClassNode;

                        foreach (var cls in assoc.Classes)
                        {
                            if (nodes.ContainsKey(cls.ClassId))
                            {
                                var fromNode = nodes[cls.ClassId];
                                var toNode = assocClassNode;

                                var link = diagram.Factory.CreateDiagramLink(fromNode, toNode);
                                link.Text = $"{cls.ClassName} : {cls.Multiplicity} \n\n\n{assoc.Name}\n ";
                                link.HeadShapeSize = 0;
                                link.BaseShapeSize = 0;     
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
                                        link.Text = $"{cls1.ClassName} : {cls1.Multiplicity} \n\n\n {assoc.Name}\n\n\n {cls2.ClassName} : {cls2.Multiplicity}";
                                        link.HeadShapeSize = 0;
                                        link.BaseShapeSize = 0;
                                        link.TextAlignment = StringAlignment.Far;
                                        
                                    }
                                }
                            }
                        }
                    }
                }

                // Arrange the diagram
                var layout = new LayeredLayout
                {
                    LayerDistance = 100, // Reduced layer distance for compactness
                    NodeDistance = 80 // Reduced node distance for compactness
                };
                layout.Arrange(diagram);
                diagram.ResizeToFitItems(10); // Reduced margin to fit diagram size
            }
        }

        /*private void CreateDiagram(List<ClassModel> classList, List<AssociationModel> associationList)
        {
            if (diagramView != null && diagramView.Diagram != null)
            {
                var diagram = diagramView.Diagram;
                diagram.ClearAll();

                var nodes = new Dictionary<string, ShapeNode>();
                var processedAssociations = new HashSet<string>(); // Untuk menghindari duplikasi asosiasi

                // Proses kelas
                foreach (var cls in classList)
                {
                    int x = (cls.ClassId.GetHashCode() % 2 == 0) ? 100 : 300;
                    int y = (cls.ClassId.GetHashCode() / 2) * 100 + 10; // Tingkatkan jarak vertikal untuk kerapian

                    var currentNode = diagram.Factory.CreateShapeNode(x, y, 80, 60); // Ukuran node diperkecil
                    currentNode.Text = $"Class ID: {cls.ClassId}\nClass Name: {cls.ClassName}\nKL: {cls.KL}";

                    if (cls.Attributes.Any())
                    {
                        currentNode.Text += "\nAttributes:";
                        foreach (var attr in cls.Attributes)
                        {
                            currentNode.Text += $"\n- {attr.AttributeType}: {attr.AttributeName} ({attr.DataType})";
                        }
                    }

                    nodes[cls.ClassId] = currentNode;
                }

                // Proses asosiasi
                foreach (var assoc in associationList)
                {
                    if (assoc.AssociationClass != null)
                    {
                        var assocClass = assoc.AssociationClass;
                        int x = (assocClass.ClassId.GetHashCode() % 2 == 0) ? 200 : 400;
                        int y = (assocClass.ClassId.GetHashCode() / 2) * 100 + 50; // Tingkatkan jarak vertikal untuk kerapian

                        var assocClassNode = diagram.Factory.CreateShapeNode(x, y, 80, 60); // Ukuran node diperkecil
                        assocClassNode.Text = $"Assoc Class ID: {assocClass.ClassId}\nClass Name: {assocClass.ClassName}\nKL: {assocClass.KL}";

                        if (assocClass.Attributes.Any())
                        {
                            assocClassNode.Text += "\nAttributes:";
                            foreach (var attr in assocClass.Attributes)
                            {
                                assocClassNode.Text += $"\n- {attr.AttributeType}: {attr.AttributeName} ({attr.DataType})";
                            }
                        }

                        nodes[assocClass.ClassId] = assocClassNode;
                    }

                    foreach (var cls in assoc.Classes)
                    {
                        var assocKey = assoc.AssociationClass != null
                            ? $"{assoc.Name}-{cls.ClassId}-{assoc.AssociationClass.ClassId}"
                            : $"{assoc.Name}-{cls.ClassId}";

                        if (!processedAssociations.Contains(assocKey))
                        {
                            processedAssociations.Add(assocKey);

                            if (assoc.AssociationClass != null)
                            {
                                if (nodes.ContainsKey(cls.ClassId) && nodes.ContainsKey(assoc.AssociationClass.ClassId))
                                {
                                    var fromNode = nodes[cls.ClassId];
                                    var toNode = nodes[assoc.AssociationClass.ClassId];

                                    var link = diagram.Factory.CreateDiagramLink(fromNode, toNode);
                                    link.Text = $"Association: {assoc.Name}";
                                    link.HeadShapeSize = 0;
                                    link.BaseShapeSize = 0;
                                }
                            }
                            else
                            {
                                foreach (var otherCls in assoc.Classes)
                                {
                                    if (cls.ClassId != otherCls.ClassId)
                                    {
                                        var linkKey = $"{cls.ClassId}-{otherCls.ClassId}";
                                        if (!processedAssociations.Contains(linkKey))
                                        {
                                            processedAssociations.Add(linkKey);

                                            if (nodes.ContainsKey(cls.ClassId) && nodes.ContainsKey(otherCls.ClassId))
                                            {
                                                var fromNode = nodes[cls.ClassId];
                                                var toNode = nodes[otherCls.ClassId];

                                                var link = diagram.Factory.CreateDiagramLink(fromNode, toNode);
                                                link.Text = $"Association: {assoc.Name}";
                                                link.HeadShapeSize = 0;
                                                link.BaseShapeSize = 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Arrange the diagram
                var layout = new LayeredLayout
                {
                    LayerDistance = 100, // Kurangi jarak antar lapisan untuk merapikan diagram
                    NodeDistance = 100 // Kurangi jarak antar node untuk merapikan diagram
                };
                layout.Arrange(diagram);
                diagram.ResizeToFitItems(10); // Kurangi margin untuk menyesuaikan ukuran diagram
            }
        }*/

    }
}