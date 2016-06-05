using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CoProService
{
    [ServiceContract(CallbackContract = typeof(ICoProServiceCallback))]
    public interface ICoProService
    {
        /// <summary>
        /// Initializes the position in the project of the connected editor 
        /// </summary>
        /// <param name="file">Which file he is currently in</param>
        /// <param name="position">The position within the file</param>
        /// <param name="name">The editor's name</param>
        /// <returns>True if there are no errors, Otherwise false</returns>
        [OperationContract]
        bool IntializePosition(string file, int position, string name);

        /// <summary>
        /// Sends a connected editor's current position
        /// </summary>
        /// <param name="file">>Which file he is currently in</param>
        /// <param name="position">The position within the file</param>
        /// <param name="content">The action that was done: added text, removed text, save and etc. </param>
        /// <returns>True if there are no errors, Otherwise false</returns>
        [OperationContract]
        bool SendCaretPosition(string file, int position, string content);

        /// <summary>
        /// Clones the project to the waiting editor
        /// </summary>
        /// <param name="path">The path of the project on the admin's computer</param>
        /// <param name="projName">The name of the project</param>
        /// <param name="id">The id of the editor to send to</param>
        /// <returns>0 if there are no errors</returns>
        [OperationContract]
        int ShareProject(string path, string projName,string id);
        
        /// <summary>
        /// Gets the project
        /// </summary>
        /// <param name="name">name of the editor</param>
        /// <returns>if the getting the project was successful true, otherwise false</returns>
        [OperationContract]
        bool GetProject(string name);

        /// <summary>
        /// Sets the current editor's admin abillities
        /// </summary>
        /// <param name="adm">True if he is, False if he is not</param>
        /// <returns>True if succeeded , Otherwise false</returns>
        [OperationContract]
        bool SetAdmin(bool adm);

        /// <summary>
        /// Checks if the current editor has communication with the server
        /// </summary>
        /// <returns>True if it is successfully called</returns>
        [OperationContract]
        bool IsConnected();

        /// <summary>
        /// Gets the expected sequence number of the next message
        /// </summary>
        /// <returns>the expected sequence number of the next message</returns>
        [OperationContract]
        int GetExpectedSeq();

        /// <summary>
        /// Sets the directory of the project on the admin's computer
        /// </summary>
        /// <param name="dir">The path of the project</param>
        /// <returns>If setting was successful true, otherwise false</returns>
        [OperationContract]
        bool SetProjectDir(string dir);

        /// <summary>
        /// Asks for the updated project from admin
        /// </summary>
        [OperationContract]
        void UpdateProject();

        /// <summary>
        /// Update a specific file, by asking the admin
        /// </summary>
        /// <param name="relPath">The relative path of the file</param>
        [OperationContract]
        void UpdateSpecificFile(string relPath);

        /// <summary>
        /// Adds an item if a new one was added while editing
        /// </summary>
        /// <param name="relpath">The relative path of the file</param>
        /// <param name="content">Its content in bytes</param>
        /// <param name="name">The name of the file</param>
        /// <param name="project">Which project the file is in</param>
        [OperationContract]
        void NewItemAdded(string relpath, byte[] content, string name, string project);

        /// <summary>
        /// Removes an item if one was reomved while editing
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="project">Which project the file is in</param>
        /// <param name="isDeleted">Determines if the file was permanently deleted or only excluded from the project</param>
        [OperationContract]
        void NewItemRemoved(string name, string project, bool isDeleted);

    }

    /// <summary>
    /// The interface that the client should implement inorder to enable duplex service, and calls from the service the client
    /// </summary>
    public interface ICoProServiceCallback
    {
        /// <summary>
        /// Gets the current list of editors and their position from the server
        /// </summary>
        /// <param name="editors">their special id</param>
        /// <param name="locations">their locations in the files and within them</param>
        /// <param name="names">their names</param>
        [OperationContract]
        void AddCurrentEditors(string[] editors, string[] locations, string[] names);

        /// <summary>
        /// An event of new editor that was added
        /// </summary>
        /// <param name="file">New editor current file</param>
        /// <param name="position">his position within the file</param>
        /// <param name="editor">his special id</param>
        /// <param name="seq">the sequence number of the message</param>
        /// <param name="name">his name</param>
        [OperationContract]
        void NewEditorAdded(string file, int position, string editor, int seq, string name);

        /// <summary>
        /// Editor disconnected event
        /// </summary>
        /// <param name="editor">The id of the disconnected editor</param>
        [OperationContract]
        void EditorDisconnected(string editor);

        /// <summary>
        /// Event for changing one editor's position
        /// </summary>
        /// <param name="file">The file</param>
        /// <param name="position">Its position in the file</param>
        /// <param name="editor">his id</param>
        /// <param name="seq">the sequence of the message</param>
        [OperationContract]
        void ChangedCaret(string file, int position, string editor, int seq);

        /// <summary>
        /// New text was added event
        /// </summary>
        /// <param name="file">The file that new text was added in</param>
        /// <param name="position">The position to add the text</param>
        /// <param name="editor">the adding editor</param>
        /// <param name="content">the actual text</param>
        /// <param name="seq">the message sequence</param>
        [OperationContract]
        void NewAddedText(string file, int position, string editor, string content, int seq);

        /// <summary>
        /// Removed text event
        /// </summary>
        /// <param name="file">The file that text was removed in</param>
        /// <param name="position">The position to removed the text</param>
        /// <param name="editor">the removing editor</param>
        /// <param name="instruc">how much to delete</param>
        /// <param name="seq">the message sequence</param>
        [OperationContract]
        void NewRemovedText(string file, int position, string editor, string instruc, int seq);

        /// <summary>
        /// Save a file/the project
        /// </summary>
        /// <param name="file">the file to save</param>
        [OperationContract]
        void Save(string file);

        /// <summary>
        /// Cloning function to send the project to the editor
        /// </summary>
        /// <param name="fileName">the project name</param>
        /// <param name="zipFile">the content in bytes</param>
        [OperationContract]
        void CloneProject(string fileName, byte[] zipFile);

        /// <summary>
        /// Approves the cloning or not by the admin decision
        /// </summary>
        /// <param name="nameToApprove"></param>
        /// <param name="idToApprove"></param>
        /// <returns>admin's decision - true if approves, otherwise false</returns>
        [OperationContract]
        bool ApproveCloning(string nameToApprove,string idToApprove);

        /// <summary>
        /// Callback for getting files to send update to
        /// </summary>
        /// <param name="file">the file of timestamps to compare the times of editing</param>
        /// <returns>the files that are not the same as the file</returns>
        [OperationContract]
        string[][] UpdateProjFilesCallback(string file);

        /// <summary>
        /// Sends the contents of files to update
        /// </summary>
        /// <param name="files">files names</param>
        /// <param name="contents">the contents</param>
        /// <param name="n_files">new files names to add</param>
        /// <param name="n_contents">new files contents</param>
        [OperationContract]
        void UpdateProjFilesContents(string[] files, byte[][] contents, string[] n_files, byte[][] n_contents);

        /// <summary>
        /// Updatesa specific file
        /// </summary>
        /// <param name="content">the content</param>
        /// <param name="relPath">the relative path of the file</param>
        [OperationContract]
        void UpdateSpecificFileCallback(byte[] content, string relPath);

        /// <summary>
        /// Open file on the admin's client
        /// </summary>
        /// <param name="file">the file's name</param>
        [OperationContract]
        void AdminFileOpen(string file);

        /// <summary>
        /// Add the new item that was added while editing to everyone
        /// </summary>
        /// <param name="relpath">the relative path of the file</param>
        /// <param name="content">its content</param>
        /// <param name="name">the name</param>
        /// <param name="project">which project the files belongs to</param>
        [OperationContract]
        void NewItemAddedCallback(string relpath, byte[] content, string name, string project);

        /// <summary>
        /// Removes an item if one was reomved while editing to every editor
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="project">Which project the file is in</param>
        /// <param name="isDeleted">Determines if the file was permanently deleted(true) or only excluded from the project(false)</param>
        [OperationContract]
        void NewItemRemovedCallback(string name, string project, bool isDeleted);
    }
}
