﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LPG_Management_System.View.Windows
{
    public partial class PrintPreview : Window
    {
        public PrintPreview(FixedDocument fixedDocument)
        {
            InitializeComponent();
            documentViewer.Document = fixedDocument;
        }
    }
}
