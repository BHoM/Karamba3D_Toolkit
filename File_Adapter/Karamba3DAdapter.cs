/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.Adapter;
using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace BH.Adapter.Karamba3D
{
    public partial class Karamba3DAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Initialises the File_Adapter without a target location. Allows to target multiple files. Target file locations will have to be specified in the Adapter Action.")]
        public Karamba3DAdapter()
        {
            // By default, if they exist already, the files to be created are wiped out and then re-created.
            this.m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.DeleteThenCreate;
        }

        [Description("Initialises the File_Adapter with a target location." +
            "\nWhen Pushing, this is used for pushing objects that are not BHoM `File` or `Directory`, like generic objects." +
            "\nWhen Pulling, if no request is specified, a FileContentRequest is automatically generated with this location.")]
        [Input("targetLocation", "FilePath, including file extension.")]
        public Karamba3DAdapter(string targetLocation)
        {
            Init(targetLocation);
        }

        [Description("Initialises the File_Adapter with a target location." +
           "\nWhen Pushing, this is used for pushing objects that are not BHoM `File` or `Directory`, like generic objects." +
           "\nWhen Pulling, if no request is specified, a FileContentRequest is automatically generated with this location.")]
        [Input("folder", "Folder path.")]
        [Input("fileName", "File name, including file extension.")]
        public Karamba3DAdapter(string folder, string fileName)
        {
            if (folder?.Count() > 2 && folder?.ElementAt(1) != ':')
                folder = Path.Combine(@"C:\ProgramData\BHoM\DataSets", folder);

            string location = Path.Combine(folder, fileName);

            Init(location);
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private bool m_Push_enableDeleteWarning = true;
        private bool m_Remove_enableDeleteWarning = true;
        private bool m_Remove_enableDeleteAlert = true;
        private bool m_Execute_enableWarning = true;
        private string m_defaultFilePath = null;


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Initialisation method for when the File Adapter is instantiated with a location.
        private bool Init(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                BH.Engine.Base.Compute.RecordError("Please specifiy a valid target location.");
                return true;
            }

            m_defaultFilePath = location;

            if (!ProcessExtension(ref m_defaultFilePath))
                return false;

            BH.Engine.Base.Compute.RecordNote($"The adapter will always target the input location `{location}`." +
                $"\nTo target multiple Files, use the {this.GetType().Name} constructor with no input.");

            // By default, the objects are appendend to the file if it exists already.
            this.m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.CreateOnly;

            return true;
        }

        /***************************************************/

        // Checks on the file extension.
        private bool ProcessExtension(ref string filePath)
        {
            string ext = Path.GetExtension(filePath);

            if (!Path.HasExtension(m_defaultFilePath))
            {
                Engine.Base.Compute.RecordNote($"No extension specified in the FileName input. Defaulting to .json.");
                ext = ".json";
                filePath += ext;
            }

            if (ext != ".json")
            {
                Engine.Base.Compute.RecordError($"File_Adapter currently supports only .json extension type.\nSpecified file extension is `{ext}`.");
                return false;
            }

            return true;
        }

        /***************************************************/
    }
}


