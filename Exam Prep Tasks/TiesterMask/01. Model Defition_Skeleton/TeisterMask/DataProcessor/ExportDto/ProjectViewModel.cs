namespace TeisterMask.DataProcessor.ExportDto
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("Project")]
    public class ProjectViewModel
    {
        [XmlAttribute("TasksCount")]
        public int TasksCount { get; set; }

        [XmlElement("ProjectName")]
        public string ProjectName { get; set; }

        [XmlElement("HasEndDate")]
        public string HasEndDate { get; set; }

        [XmlArray("Tasks")]
        public List<TaskViewModel> Tasks { get; set; }
    }

    [XmlType("Task")]
    public class TaskViewModel
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Label")]
        public string Label { get; set; }
    }
}
