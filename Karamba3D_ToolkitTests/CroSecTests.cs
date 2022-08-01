using System;
using System.Collections.Generic;
using Karamba.CrossSections;
using Karamba.Utilities;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BH.Engine.Adapters.Karamba3D;
using BH.oM.Structure.SectionProperties;

namespace Karamba3D_ToolkitTests
{
    public class CroSecTests
    {
        [Test]
        public void Concreteconversion()
        { 
            var table = new CroSecTable();
            var defaultCrossSectionPath = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location,"../../../../","ExternalFile/CrossSectionValues.bin"));
            table.read(defaultCrossSectionPath);

            var test = table.crosecs.OfType<CroSec_Beam>().Where(c => Math.Abs(c.Wely_z_pos + c.Wely_z_neg) > 1E-12).ToList();
            var families = test.Select(c => c.family).ToHashSet();

            var hea180 = table.crosecs.Single(c => c.name == "HEA180");

            var bhomSection = hea180.ToBHoM();

            var mapper = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"BHoM\Datasets\Karamba3D\Karamba3DToBhOMCrossSectionMapper.csv");

        }
    }
}