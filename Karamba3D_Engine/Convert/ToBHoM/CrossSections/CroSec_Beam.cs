using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using Karamba.CrossSections;
using BH.Engine.Spatial;
using BH.Engine.Structure;
using feb;

namespace BH.Engine.Adapters.Karamba3D
{
    using Adapter.Karamba3D;
    using Karamba3D_Engine;

    public static partial class Convert
    {
        public static ISectionProperty ToBhOM(this CroSec_Beam k3dCrossSection)
        {
            if (k3dCrossSection is null)
                return null;

            if (TryGetSectionFromDataSet(k3dCrossSection.name, out var databaseSection))
            {
                return databaseSection;
            }

            var material = k3dCrossSection.material.ToBhOM();
            var profile = CreateProfile(k3dCrossSection);

            if (profile != null)
            {
                var section  = Engine.Structure.Create.SectionPropertyFromProfile(profile, material, k3dCrossSection.name);
                // TODO the set adapter id has to be setup?
                // section.SetAdapterId();
                return section;
            }
            else
            {
                return new ExplicitSection()
                {
                    Name = k3dCrossSection.name,
                    Material = material,
                    Area = k3dCrossSection.A,
                    Rgy = k3dCrossSection.iy,
                    Rgz = k3dCrossSection.iz,
                    J = k3dCrossSection.Ipp,
                    Iy = k3dCrossSection.Iyy,
                    Iz = k3dCrossSection.Izz,
                    Iw = k3dCrossSection.Cw,
                    Wely = Math.Min(Math.Abs(k3dCrossSection.Wely_z_neg), k3dCrossSection.Welz_y_pos),
                    Welz = Math.Min(Math.Abs(k3dCrossSection.Welz_y_neg), k3dCrossSection.Wely_z_pos),
                    Wply = k3dCrossSection.Wply,
                    Wplz = k3dCrossSection.Wplz,
                    Vz = k3dCrossSection.zs,
                    Vpz = k3dCrossSection.getHeight() - k3dCrossSection.zs,
                    Asy = k3dCrossSection.Ay,
                    Asz = k3dCrossSection.Az,
                };
            }
        }

        private static bool TryGetSectionFromDataSet(string k3dSectionName, out ISectionProperty bhomSection)
        {
            bhomSection = null;

            // TODO create a map for database materials.
            var crossSectionMapPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "BHoM",
                "Datasets",
                "Karamba3D",
                "Karamba3DToBhOMCrossSectionMapper.csv");
            
            // Search for file
            if (!File.Exists(crossSectionMapPath))
            {
                var message = string.Format(
                    Resource.ErrorCrossSectionMapNotFound,
                    Path.GetFullPath(crossSectionMapPath));

                throw new FileNotFoundException(message);
            }

            var csvRow = File.ReadLines(crossSectionMapPath)
                                    .Select(l => l.Split(';'))
                                    .FirstOrDefault(r => r[0] == k3dSectionName);

            if (csvRow is null)
            {
                return false;
            }

            var dataSetName = csvRow[1];
            var sectionName = csvRow[2];
            var test = Compute.GetDatasetData<SteelSection>(dataSetName);
            bhomSection = test.FirstOrDefault(s => s.Name == sectionName);

            return bhomSection != null;

        }

        private static IProfile CreateProfile(CroSec_Beam obj)
        {
            switch (obj)
            {
                case CroSec_Circle circle:
                {
                    var diameter = circle.getHeight();
                    var thickness = circle.thick;
                    
                    // If the thickness is less then zero or is higher then half diameter,
                    // a full circle will be created. 
                    return thickness <= 0 || thickness >= diameter / 2 ? 
                        (IProfile)Spatial.Create.CircleProfile(diameter) :
                        Spatial.Create.TubeProfile(diameter, thickness);
                }

                case CroSec_T tSection:
                {
                    return Spatial.Create.TSectionProfile(
                        tSection._height,
                        tSection.uf_width,
                        tSection.w_thick,
                        tSection.uf_thick,
                        tSection.fillet_r);
                }

                case CroSec_Box box:
                {
                    if (Math.Abs(box.uf_width - box.lf_width) > double.Epsilon)
                    {
                        K3dLogger.RecordError(Resource.ErrorDifferentFlangeNotSupported);
                    }

                    // TODO use a global tolerance for Karamba!
                    if (Math.Abs(box.uf_thick - box.lf_thick) > double.Epsilon ||
                        Math.Abs(box.w_thick - box.uf_thick) > double.Epsilon)
                    {
                        if (box.fillet_r > 0 || box.fillet_r1 > 0)
                        {
                            K3dLogger.RecordWarning(Resource.WarningBoxCrossSectionNotSupportedFillet);
                        }

                        return Spatial.Create.FabricatedISectionProfile(
                            box._height,
                            box.uf_width,
                            box.lf_width,
                            box.w_thick,
                            box.uf_thick,
                            box.lf_thick);
                    }
                    else
                    {
                        return Spatial.Create.BoxProfile(
                            box._height,
                            box.uf_width,
                            box.w_thick,
                            box.fillet_r1,
                            box.fillet_r);
                    }
                }

                case CroSec_I iSection:
                {
                    if (Math.Abs(iSection.uf_width - iSection.lf_width) > double.Epsilon ||
                        Math.Abs(iSection.uf_thick - iSection.lf_thick) > double.Epsilon)
                    {
                        if (iSection.fillet_r > 0)
                        {

                            K3dLogger.RecordWarning(Resource.WarningICrossSectionNotSupportedFillet);
                        }


                        return Spatial.Create.FabricatedISectionProfile(
                            iSection._height,
                            iSection.uf_width,
                            iSection.lf_width,
                            iSection.w_thick,
                            iSection.uf_thick,
                            iSection.lf_thick);
                    }
                    else
                    {
                        return Spatial.Create.ISectionProfile(
                            iSection._height,
                            iSection.uf_width,
                            iSection.w_thick,
                            iSection.uf_thick,
                            iSection.fillet_r,
                            0D);
                    }
                }

                default:
                {
                    var message = string.Format(
                        Resource.ErrorCrossSectionProfileConversion,
                        typeof(IProfile),
                        obj.GetType().FullName);

                    K3dLogger.RecordError(message);
                    return null;
                }
            }
        }
    }
}