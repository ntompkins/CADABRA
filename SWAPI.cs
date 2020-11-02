using System;
using SolidWorks.Interop.swdocumentmgr;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.Linq;

public class SwApi
{
    // Object properties
    public int FileVersion     { get; set; }
    public string HashCode     { get; set; }
    public string SavedByUser  { get; set; }
    public string LastSaveDate { get; set; }
    public string CreationDate { get; set; }
    public string WorkTime     { get; set; }
    public bool Flagged        { get; set; }

    // Retreive API key from environment variable
    public string APIKey = Environment.GetEnvironmentVariable("SW_API_KEY");

    // Enumerated file type
    SwDmDocumentType FileType;

    public SwApi(string FilePath, string SWType, string[] RuleList)
    {
        // Declare class type for SW API
        SwDMClassFactory classFactory = Activator.CreateInstance(
            Type.GetTypeFromProgID("SwDocumentMgr.SwDMClassFactory")
        ) as SwDMClassFactory;

        if (classFactory != null)
        {
            // Create app instance
            SwDMApplication dmApp = classFactory.GetApplication(APIKey);
            FileVersion = dmApp.GetLatestSupportedFileVersion();

            // Determine file type for actions
            switch(SWType.ToLower())
            {
                // Part file type
                case "prt":
                    FileType = SwDmDocumentType.swDmDocumentPart;
                    break;

                // Assembly file type
                case "asm":
                    FileType = SwDmDocumentType.swDmDocumentAssembly;
                    break;

                // Throw error for incompatible type
                default:
                    throw new Exception("Error, incompatible file type from file: " + FilePath);
            }

            // Create error object
            SwDmDocumentOpenError err;
            SwDMDocument doc = dmApp.GetDocument(FilePath, FileType, true, out err);

            if(err == SwDmDocumentOpenError.swDmDocumentOpenErrorNone)
            {
                // Grab metadata from file
                HashCode = doc.GetHashCode().ToString();
                SavedByUser = doc.LastSavedBy;
                LastSaveDate = doc.LastSavedDate;
                CreationDate = doc.CreationDate;
            }
            else
            {
                throw new Exception("Error creating object from path:" + FilePath);
            }

            // TODO: Apply rules for checking from <rule[]>

        }
        else
        {
            throw new NullReferenceException("Document Manager SDK is not installed");
        }
    }

    // Returns human-readable, largest unit first, time format
    string CalculateWorkTime()
    {
        // TODO: Functionality
        // Parse LastSavedDate and CreationDate
        // Find difference between
        // Return formatted string
        return "null";
    }

    // Returns true or false if author matches input
    bool CompareAuthorTo(string ComparisonName)
    {
        return ComparisonName == SavedByUser ? true : false;
    }
}
