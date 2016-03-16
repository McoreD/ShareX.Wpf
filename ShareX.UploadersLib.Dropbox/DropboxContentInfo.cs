using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib.Dropbox
{
    public class DropboxContentInfo
    {
        public string Size { get; set; } // A human-readable description of the file size (translated by locale).
        public long Bytes { get; set; } // The file size in bytes.
        public string Path { get; set; } // Returns the canonical path to the file or directory.
        public bool Is_dir { get; set; } // Whether the given entry is a folder or not.
        public bool Is_deleted { get; set; } // Whether the given entry is deleted (only included if deleted files are being returned).
        public string Rev { get; set; } // A unique identifier for the current revision of a file. This field is the same rev as elsewhere in the API and can be used to detect changes and avoid conflicts.
        public string Hash { get; set; } // A folder's hash is useful for indicating changes to the folder's contents in later calls to /metadata. This is roughly the folder equivalent to a file's rev.
        public bool Thumb_exists { get; set; } // True if the file is an image can be converted to a thumbnail via the /thumbnails call.
        public string Icon { get; set; } // The name of the icon used to illustrate the file type in Dropbox's icon library.
        public string Modified { get; set; } // The last time the file was modified on Dropbox, in the standard date format (not included for the root folder).
        public string Client_mtime { get; set; } // For files, this is the modification time set by the desktop client when the file was added to Dropbox, in the standard date format. Since this time is not verified (the Dropbox server stores whatever the desktop client sends up), this should only be used for display purposes (such as sorting) and not, for example, to determine if a file has changed or not.
        public string Root { get; set; } // The root or top-level folder depending on your access level. All paths returned are relative to this root level. Permitted values are either dropbox or app_folder.
        public long Revision { get; set; } // A deprecated field that semi-uniquely identifies a file. Use rev instead.
        public string Mime_type { get; set; }
        public DropboxContentInfo[] Contents { get; set; }
    }
}