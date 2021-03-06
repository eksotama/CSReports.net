﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSKernelClient;

namespace CSKernelFile
{
    public class cFileEx
    {

        private const String C_MODULE = "cFileEx";

        public String fileGetName(String fullPath)
        {
            return getFileNameWithoutExt(fullPath);
        }

        public bool fileExists(String file)
        {
            return File.Exists(file);
        }

        public String getWindowsDir()
        {
            return System.Environment.SystemDirectory;
        }

        public String fileGetPath(String fullPath)
        {
            String path = "";
            String fileName = "";

            separatePathAndFileName(fullPath, ref path, ref fileName);

            return path;
        }

        public String fileGetFileExt(String fullPath)
        {
            String path = "";
            String fileName = "";
            int pos = 0;
            String c = "";

            separatePathAndFileName(fullPath, ref path, ref fileName);

            pos = fileName.Length;

            if (pos == 0)
            {
                return "";
            }

            c = fileName.Substring(pos, 1);
            while (c != ".")
            {
                pos = pos - 1;
                if (pos == 0) { break; }
                c = fileName.Substring(pos, 1);
            }

            switch (pos)
            {
                case 0:
                    // if there is not a separator this file doesn't have extension
                    //
                    return "";

                default:
                    // return the extension
                    //
                    return fileName.Substring(pos + 1);
            }
        }

        public void fileGetPathAndFileName(String fullPath, ref String path, ref String fileName)
        {
            separatePathAndFileName(fullPath, ref path, ref fileName);
        }

        public bool fileCopyFile(String source, String destination)
        {
            try
            {
                File.Copy(source, destination);
                return true;
            }
            catch (Exception ex) 
            {
                cError.mngError(ex, "fileCopyFile", C_MODULE, "source: " + source + "\ndestination:" + destination);
                return false;
            }
        }

        public bool fileDelete(String file)
        {
            try
            {
                if (fileExists(file))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "fileDelete", C_MODULE, "file: " + file);
                return false;
            }
        }

        public static String getFileNameWithoutExt(String fullPath)
        {
            String path = "";
            String fileName = "";
            int pos = 0;
            String sep = "";

            separatePathAndFileName(fullPath, ref path, ref fileName);
            pos = fileName.Length;

            if (pos == 0)
            {
                return fullPath;
            }

            sep = fileName.Substring(pos, 1);
            while (sep != ".")
            {
                pos = pos - 1;
                if (pos == 0) { break; }
                sep = fileName.Substring(pos, 1);
            }

            switch (pos)
            {
                case 0:
                    return fileName;

                default:
                    return fileName.Substring(0, pos - 1);
            }
        }

        public static void separatePathAndFileName(String fullPath, ref String path, ref String fileName)
        {
            int pos = 0;
            String sep = "";

            pos = fullPath.Length;

            if (pos == 0)
            {
                path = fullPath;
                fileName = fullPath;
                return;
            }
            sep = fullPath.Substring(pos, 1);
            while (!isSeparator(sep))
            {
                pos = pos - 1;
                if (pos == 0) { break; }
                sep = fullPath.Substring(pos, 1);
            }

            if (pos == fullPath.Length-1)
            {
                // if the separator is founded at the end it must be a root folder example: c:\
                //
                path = fullPath.Substring(0, pos - 1);
                fileName = fullPath;
            }
            else if (pos == 0)
            {
                // if the separator is not found it must be a root folder example: c:
                //
                path = fullPath;
                fileName = fullPath;
            }
            else
            {
                path = fullPath.Substring(0, pos - 1);
                fileName = fullPath.Substring(pos + 1);
            }
        }

        private static bool isSeparator(String character)
        {
            if (character == "\\")
            {
                return true;
            }
            if (character == "/")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
