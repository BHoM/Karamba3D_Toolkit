/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.Karamba3D
{
    [Description("Container of the BHoMObjects converted from the Karamba model.")]
    public class FemModel : Base.IContainer
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        [Description("Nodes converted from the Karamba model.")]
        public virtual IList<Node> Nodes { get; set; } = new List<Node>();

        [Description("Bars converted from the Karamba model.")]
        public virtual IList<Bar> Bars { get; set; } = new List<Bar>();

        [Description("Loads converted from the Karamba model.")]
        public virtual IList<ILoad> Loads { get; set; } = new List<ILoad>();

        [Description("Load cases converted from the Karamba model.")]
        public virtual IList<Loadcase> LoadCases { get; set; } = new List<Loadcase>();

        [Description("Cross sections converted from the Karamba model.")]
        public virtual IList<ISectionProperty> CrossSections { get; set; } = new List<ISectionProperty>();

        [Description("Materials converted from the Karamba model.")]
        public virtual IList<IMaterialFragment> Materials { get; set; } = new List<IMaterialFragment>();
    }
}