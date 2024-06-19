using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;


namespace xtUML1 {


    public class StateDiagramVisualizer
    {
        public void Visualize(string jsonContent)
        {
            var graph = new Graph("state diagram");
            var jsonObj = JObject.Parse(jsonContent);

            // Contoh asumsi struktur JSON: {"states": ["A", "B"], "transitions": [{"from": "A", "to": "B", "label": "go"}]}
            var states = jsonObj["states"];
            var transitions = jsonObj["transitions"];

            if (states != null)
            {
                foreach (var state in states)
                {
                    graph.AddNode(state.ToString());
                }
            }

            if (transitions != null)
            {
                foreach (var transition in transitions)
                {
                    var from = transition["from"].ToString();
                    var to = transition["to"].ToString();
                    var label = transition["label"]?.ToString();
                    var edge = graph.AddEdge(from, to);
                    if (label != null)
                    {
                        edge.LabelText = label;
                    }
                }
            }

            var form = new Form
            {
                Text = "State Diagram Visualization",
                Width = 800,
                Height = 600
            };

            var viewer = new GViewer
            {
                Graph = graph,
                Dock = System.Windows.Forms.DockStyle.Fill
            };

            form.Controls.Add(viewer);
            form.ShowDialog();
        }
    }

}
}

