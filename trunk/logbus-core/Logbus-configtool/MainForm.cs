/*
 *                  Logbus-ng project
 *    ©2010 Logbus Reasearch Team - Some rights reserved
 *
 *  Created by:
 *      Vittorio Alfieri - vitty85@users.sourceforge.net
 *      Antonio Anzivino - djechelon@users.sourceforge.net
 *
 *  Based on the research project "Logbus" by
 *
 *  Dipartimento di Informatica e Sistemistica
 *  University of Naples "Federico II"
 *  via Claudio, 21
 *  80121 Naples, Italy
 *
 *  Software is distributed under Microsoft Reciprocal License
 *  Documentation under Creative Commons 3.0 BY-SA License
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace It.Unina.Dis.Logbus.Configtool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void mnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Logbus-ng configuration tool.\n\nCopyright ©2010 the Logbus Research Team\nUniversity of Naples \"Federico II\" - Dipartimento di Informatica e Sistemistica\nvia Claudio, 21 - 80121 Naples, Italy", "About");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateConfiguration();
        }


        private void UpdateConfiguration()
        {
            XmlDocument AppConfig = new XmlDocument();
            AppConfig.AppendChild(AppConfig.CreateXmlDeclaration("1.0", "utf-8", string.Empty));

            XmlElement configuration = AppConfig.CreateElement("configuration");
            AppConfig.AppendChild(configuration);

            XmlElement configsections = AppConfig.CreateElement("configSections");
            configuration.AppendChild(configsections);

            {
                XmlElement configsection_core = AppConfig.CreateElement("section");
                configsections.AppendChild(configsection_core);

                XmlAttribute attr_section_core_name = AppConfig.CreateAttribute("name");
                attr_section_core_name.Value = "logbus-core";
                configsection_core.Attributes.Append(attr_section_core_name);

                XmlAttribute attr_section_core_type = AppConfig.CreateAttribute("type");
                attr_section_core_type.Value = typeof(It.Unina.Dis.Logbus.Configuration.LogbusServerConfigurationSectionHandler).AssemblyQualifiedName;
                configsection_core.Attributes.Append(attr_section_core_type);
            }

            {
                XmlElement configsection_client = AppConfig.CreateElement("section");
                configsections.AppendChild(configsection_client);

                XmlAttribute attr_section_core_name = AppConfig.CreateAttribute("name");
                attr_section_core_name.Value = "logbus-client";
                configsection_client.Attributes.Append(attr_section_core_name);

                XmlAttribute attr_section_core_type = AppConfig.CreateAttribute("type");
                attr_section_core_type.Value = typeof(It.Unina.Dis.Logbus.Configuration.LogbusClientConfigurationSectionHandler).AssemblyQualifiedName;
                configsection_client.Attributes.Append(attr_section_core_type);
            }

            {
                XmlElement configsection_source = AppConfig.CreateElement("section");
                configsections.AppendChild(configsection_source);

                XmlAttribute attr_section_core_name = AppConfig.CreateAttribute("name");
                attr_section_core_name.Value = "logbus-source";
                configsection_source.Attributes.Append(attr_section_core_name);

                XmlAttribute attr_section_core_type = AppConfig.CreateAttribute("type");
                attr_section_core_type.Value = typeof(It.Unina.Dis.Logbus.Configuration.LogbusLoggerConfigurationSectionHandler).AssemblyQualifiedName;
                configsection_source.Attributes.Append(attr_section_core_type);
            }

            new XmlShowForm() { Node = AppConfig }.Show(this);
        }

    }
}
