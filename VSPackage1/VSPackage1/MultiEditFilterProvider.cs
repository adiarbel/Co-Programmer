﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Company.VSPackage1
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class MultiEditFilterProvider : IVsTextViewCreationListener//classifies as an extension
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("MultiEditLayer")]
        [TextViewRole(PredefinedTextViewRoles.Editable)]
        internal AdornmentLayerDefinition m_multieditAdornmentLayer = null;
        [Import(typeof(IVsEditorAdaptersFactoryService))]
        internal IVsEditorAdaptersFactoryService editorFactory = null;
        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView textView = editorFactory.GetWpfTextView(textViewAdapter);//gets the text view
            if (textView == null)
                return;

            AddCommandFilter(textViewAdapter, new MultiEditCommandFilter(textView));//adds an instance of our command filter to the text view
        }
        void AddCommandFilter(IVsTextView viewAdapter, MultiEditCommandFilter commandFilter)
        {
            if (commandFilter.m_added == false)
            {
                //get the view adapter from the editor factory
                IOleCommandTarget next;
                int hr = viewAdapter.AddCommandFilter(commandFilter, out next);

                if (hr == VSConstants.S_OK)
                {
                    commandFilter.m_added = true;
                    //you'll need the next target for Exec and QueryStatus
                    if (next != null)
                        commandFilter.m_nextTarget = next;
                }
            }
        }
    }
}
