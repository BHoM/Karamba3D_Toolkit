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
using Log = BH.Engine.Adapter.Karamba3D.Log;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        public static ISectionProperty ToBHoM(this CroSec_Beam obj)
        {
            if (obj is null)
                return null;

            if (TryGetSectionFromDataSet(obj.name, out var databaseSection))
            {
                return databaseSection;
            };

            var material = obj.material.ToBHoM();
            var profile = CreateProfile(obj);

            if (profile != null)
            {
                var section  = Engine.Structure.Create.SectionPropertyFromProfile(profile, material, obj.name);
                // TODO the set adapter id has to be setup?
                // section.SetAdapterId();
                return section;
            }
            else
            {
                return new ExplicitSection()
                {
                    Name = obj.name,
                    Material = material,
                    Area = obj.A,
                    Rgy = obj.iy,
                    Rgz = obj.iz,
                    J = obj.Ipp,
                    Iy = obj.Iyy,
                    Iz = obj.Izz,
                    Iw = obj.Cw,
                    Wely = Math.Min(Math.Abs(obj.Wely_z_neg), obj.Welz_y_pos),
                    Welz = Math.Min(Math.Abs(obj.Welz_y_neg), obj.Wely_z_pos),
                    Wply = obj.Wply,
                    Wplz = obj.Wplz,
                    Vz = obj.zs,
                    Vpz = obj.getHeight() - obj.zs,
                    Asy = obj.Ay,
                    Asz = obj.Az,
                };
            }

            var test = obj.alpha_lt;
            ExplicitSection es = new ExplicitSection();
            es.CustomData[nameof(obj.Ay)] = obj.Ay;


        }

        private static bool TryGetSectionFromDataSet(string k3dSectionName, out ISectionProperty bhomSection)
        {
            bhomSection = null;

            // TODO create a map for database materials.
            
            // Search for file
            var mapperPath = @"C:\ProgramData\BHoM\Datasets\Karamba3D\Karamba3DToBhOMCrossSectionMapper.csv";

            if (!File.Exists(mapperPath))
            {
                throw new FileNotFoundException($"{mapperPath} was not found.");
            }

            var csvRow = File.ReadLines(mapperPath)
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
                // TODO maybe remove this switch case.
                case CroSec_BeamModifier beamModifier:
                    throw new ArgumentException($"Conversion from {typeof(CroSec_BeamModifier)} is not supported.");

                case CroSec_Circle circle:
                {
                    // TODO remove the getHeight from the circle and use a property
                    var diameter = circle.getHeight();
                    var thickness = circle.thick;
                    
                    // If the thickness less then zero or is higher then half diameter,
                    // a full circle will be created. 
                    return thickness <= 0 || thickness >= diameter / 2 ? 
                        (IProfile)Spatial.Create.CircleProfile(diameter) :
                        Spatial.Create.TubeProfile(diameter, thickness);
                }

                case CroSec_T tSection:
                {
                    // TODO Add fillets.
                    return Spatial.Create.TSectionProfile(
                        tSection._height,
                        tSection.uf_width,
                        tSection.w_thick,
                        tSection.uf_thick);
                }

                case CroSec_Box box:
                {
                    if (Math.Abs(box.uf_width - box.lf_width) > double.Epsilon)
                    {
                        Base.Compute.RecordError(
                            "Box cross section with different flange widths are not supported yet.");
                    }

                    // TODO use a global tolerance for Karamba!
                    if (Math.Abs(box.uf_thick - box.lf_thick) > double.Epsilon ||
                        Math.Abs(box.w_thick - box.uf_thick) > double.Epsilon)
                    {
                        if (box.fillet_r > 0 || box.fillet_r1 > 0)
                        {
                            Base.Compute.RecordWarning(
                                "The cross section fillet values are not supported when when the box cross section has different thicknesses or flange widths. The fillet values have not been exported.");
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
                            // TODO Add resource file
                            Log.RecordWarning(
                                "The cross section fillet value is not compatible with not symmetrical flanges. The value has not been exported.");
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
                    Base.Compute.RecordError($"Could not find a convert method for {obj.GetType().FullName}.");
                    return null;
                }
            }
        }
    }
}