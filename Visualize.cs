using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace xtUML1
{
    public class Visualize
    {
        private StringBuilder dataBuilder = new StringBuilder();
        public List<ClassModel> classList = new List<ClassModel>();
        public List<AssociationModel> associationList = new List<AssociationModel>();
        
        public string VisualiseJson(string text)
        {
            try
            {
                classList.Clear();
                associationList.Clear();
                var jsonString = text;
                var jsonArray = JArray.Parse(jsonString);

                foreach (var item in jsonArray)
                {
                    if (item["model"] != null)
                    {
                        foreach (var model in item["model"])
                        {
                            string type = model["type"].ToString();
                            if (type == "class")
                            {
                                ProcessClass(model);
                            }
                            else if (type == "association")
                            {
                                ProcessAssociation(model);
                            }
                        }
                    }
                }

                dataBuilder.AppendLine ("Classes:\n");
                foreach (var cls in classList)
                {
                    dataBuilder.AppendLine($"Class ID: {cls.ClassId}");
                    dataBuilder.AppendLine($"Class Name: {cls.ClassName}");
                    dataBuilder.AppendLine($"KL: {cls.KL}");
                    dataBuilder.AppendLine("Attributes:");
                    foreach (var attr in cls.Attributes)
                    {
                        dataBuilder.AppendLine($"  - {attr.AttributeType}: {attr.AttributeName} ({attr.DataType})");
                    }
                    dataBuilder.AppendLine("");
                }
                dataBuilder.AppendLine("Associations:\n");
                foreach (var assoc in associationList)
                {
                    dataBuilder.AppendLine($"Association Name: {assoc.Name}");
                    dataBuilder.AppendLine("Classes:");
                    foreach (var cls in assoc.Classes)
                    {
                        dataBuilder.AppendLine($"  - Class ID: {cls.ClassId}, Name: {cls.ClassName}, Multiplicity: {cls.Multiplicity}");
                    }

                    if (assoc.AssociationClass != null)
                    {
                        dataBuilder.AppendLine($"\nAssociation Class ID: {assoc.AssociationClass.ClassId}");
                        dataBuilder.AppendLine($"Class Name: {assoc.AssociationClass.ClassName}");
                        dataBuilder.AppendLine($"KL: {assoc.AssociationClass.KL}");
                        dataBuilder.AppendLine("Attributes:");
                        foreach (var attr in assoc.AssociationClass.Attributes)
                        {
                            dataBuilder.AppendLine($"  - {attr.AttributeType}: {attr.AttributeName} ({attr.DataType})");
                        }
                    }
                    dataBuilder.AppendLine("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            var textParsed = dataBuilder.ToString();
            return textParsed;
        }

        private void ProcessClass(JToken model)
        {
            string classId = model["class_id"]?.ToString();
            string className = model["class_name"]?.ToString();

            if (string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(className))
            {
                return;
            }

            string kl = model["KL"]?.ToString();
            var classModel = new ClassModel
            {
                ClassId = classId,
                ClassName = className,
                KL = kl,
                Attributes = new List<AttributeModel>()
            };

            foreach (var attribute in model["attributes"] ?? new JArray())
            {
                string attributeType = attribute["attribute_type"].ToString();
                string attributeName = attribute["attribute_name"].ToString();
                string dataType = attribute["data_type"].ToString();

                var attributeModel = new AttributeModel
                {///\\]]]]
                    AttributeType = attributeType,
                    AttributeName = attributeName,
                    DataType = dataType
                };

                classModel.Attributes.Add(attributeModel);
            }

            classList.Add(classModel);
        }

        private void ProcessAssociation(JToken model)
        {
            var associationModel = new AssociationModel
            {
                Name = model["name"]?.ToString(),
                Classes = new List<AssocClass>()
            };

            foreach (var assocClass in model["class"] ?? new JArray())
            {
                string assocClassId = assocClass["class_id"].ToString();
                string assocClassName = assocClass["class_name"].ToString();
                string assocClassMultiplicity = assocClass["class_multiplicity"].ToString();

                var assocClassModel = new AssocClass
                {
                    ClassId = assocClassId,
                    ClassName = assocClassName,
                    Multiplicity = assocClassMultiplicity
                };

                associationModel.Classes.Add(assocClassModel);
            }

            if (model["model"] != null && model["model"]["type"]?.ToString() == "association_class")
            {
                var assocModel = model["model"];
                string classId = assocModel["class_id"]?.ToString();
                string className = assocModel["class_name"]?.ToString();
                string kl = assocModel["KL"]?.ToString();

                var associationClassModel = new ClassModel
                {
                    ClassId = classId,
                    ClassName = className,
                    KL = kl,
                    Attributes = new List<AttributeModel>()
                };

                foreach (var attribute in assocModel["attributes"] ?? new JArray())
                {
                    string attributeType = attribute["attribute_type"].ToString();
                    string attributeName = attribute["attribute_name"].ToString();
                    string dataType = attribute["data_type"].ToString();

                    var attributeModel = new AttributeModel
                    {
                        AttributeType = attributeType,
                        AttributeName = attributeName,
                        DataType = dataType
                    };

                    associationClassModel.Attributes.Add(attributeModel);
                }

                associationModel.AssociationClass = associationClassModel;
            }

            associationList.Add(associationModel);
        }

        public class ClassModel
        {
            public string ClassId { get; set; }
            public string ClassName { get; set; }
            public string KL { get; set; }
            public List<AttributeModel> Attributes { get; set; }
        }

        public class AttributeModel
        {
            public string AttributeType { get; set; }
            public string AttributeName { get; set; }
            public string DataType { get; set; }
        }

        public class AssociationModel
        {
            public string Name { get; set; }
            public List<AssocClass> Classes { get; set; }
            public ClassModel AssociationClass { get; set; }
        }

        public class AssocClass
        {
            public string ClassId { get; set; }
            public string ClassName { get; set; }
            public string Multiplicity { get; set; }
        }

    }
}
