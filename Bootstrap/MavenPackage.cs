/*
 * Copyright 2015 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Amazon Software License (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/asl/
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using System;
using System.IO;
using System.Collections.Generic;

namespace Amazon.Kinesis.ClientLibrary.Bootstrap
{
    /// <summary>
    /// Represents a Maven java package. We need to download a bunch of these in order
    /// to use the java KCL.
    /// </summary>
    internal class MavenPackage
    {
        public readonly String GroupId;
        public readonly String ArtifactId;
        public readonly String Version;

        /// <summary>
        /// Gets the name of the jar file of this Maven package.
        /// </summary>
        /// <value>The name of the jar file.</value>
        public String FileName
        {
            get
            {
                return String.Format("{0}-{1}.jar", ArtifactId, Version);
            }
        }

        public MavenPackage(String groupId, String artifactId, String version)
        {
            GroupId = groupId;
            ArtifactId = artifactId;
            Version = version;
        }

        /// <summary>
        /// Check if the jar file for this Maven package already exists on disk.
        /// </summary>
        /// <param name="folder">Folder to look in.</param>
        public bool Exists(String folder)
        {
            return File.Exists(Path.Combine(folder, FileName));
        }

        /// <summary>
        /// Download the jar file for this Maven package.
        /// </summary>
        /// <param name="folder">Folder to download the file into.</param>
        public void Fetch(String folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            String destination = Path.Combine(folder, FileName);
            if (!File.Exists(destination))
            {
                var client = new System.Net.WebClient();
                Console.Error.WriteLine(Url + " --> " + destination);
                client.DownloadFile(new Uri(Url), destination);
            }
        }

        /// <summary>
        /// Gets the URL to the jar file for this Maven package.
        /// </summary>
        /// <value>The URL.</value>
        private String Url
        {
            get
            {
                List<String> urlParts = new List<String>();
                urlParts.AddRange(GroupId.Split('.'));
                urlParts.Add(ArtifactId);
                urlParts.Add(Version);
                urlParts.Add(FileName);
                return "http://search.maven.org/remotecontent?filepath=" + String.Join("/", urlParts);
            }
        }
    }
}
