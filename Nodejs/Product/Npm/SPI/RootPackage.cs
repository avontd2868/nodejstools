﻿//*********************************************************//
//    Copyright (c) Microsoft. All rights reserved.
//    
//    Apache 2.0 License
//    
//    You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//    
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
//    implied. See the License for the specific language governing 
//    permissions and limitations under the License.
//
//*********************************************************//

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;

namespace Microsoft.NodejsTools.Npm.SPI {
    internal class RootPackage : IRootPackage {
        public RootPackage(
            string fullPathToRootDirectory,
            bool showMissingDevOptionalSubPackages) {
            Path = fullPathToRootDirectory;
            try {
                PackageJson = PackageJsonFactory.Create(new DirectoryPackageJsonSource(fullPathToRootDirectory));
            } catch (RuntimeBinderException rbe) {
                throw new PackageJsonException(
                    string.Format(@"Error processing package.json at '{0}'. The file was successfully read, and may be valid JSON, but the objects may not match the expected form for a package.json file.

The following error was reported:

{1}",
                    System.IO.Path.Combine(fullPathToRootDirectory, "package.json"),
                    rbe.Message),
                    rbe);
            }

            try {
                Modules = new NodeModules(this, showMissingDevOptionalSubPackages);
            }  catch (PathTooLongException) {
                // otherwise we fail to create it completely...
            }
        }

        public IPackageJson PackageJson { get; private set; }

        public bool HasPackageJson {
            get { return null != PackageJson; }
        }

        public string Name {
            get { return null == PackageJson ? new DirectoryInfo(Path).Name : PackageJson.Name; }
        }

        public SemverVersion Version {
            get { return null == PackageJson ? new SemverVersion() : PackageJson.Version; }
        }

        public IPerson Author {
            get { return null == PackageJson ? null : PackageJson.Author; }
        }

        public string Description {
            get { return null == PackageJson ? null : PackageJson.Description; }
        }

        public IEnumerable<string> Homepages {
            get { return null == PackageJson ? null : PackageJson.Homepages; }
        }

        public string Path { get; private set; }

        public INodeModules Modules { get; private set; }
    }
}