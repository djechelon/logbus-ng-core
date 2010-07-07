using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace It.Unina.Dis.Logbus.Configtool
{
    public partial class XmlShowForm : Form
    {

        private XmlNode the_node;

        public XmlNode Node
        {
            protected get
            {
                return the_node;
            }
            set
            {
                the_node = value;
                Refresh();
            }
        }

        public XmlShowForm()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            txtXml.Text = "";

            if (Node != null)
            {
                using (StringWriter sw = new StringWriter())
                {
                    XmlTextWriter tw = new XmlTextWriter(sw);
                    tw.Formatting = Formatting.Indented;
                    Node.WriteTo(tw);
                    sw.Flush();
                    txtXml.Text = sw.ToString();
                }
            }

            base.Refresh();
        }
    }
}
