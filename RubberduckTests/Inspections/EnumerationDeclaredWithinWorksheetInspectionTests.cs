﻿using System.Linq;
using Rubberduck.CodeAnalysis.Inspections;
using Rubberduck.CodeAnalysis.Inspections.Concrete;
using Rubberduck.Parsing.VBA;
using Rubberduck.VBEditor.SafeComWrappers;
using NUnit.Framework;

namespace RubberduckTests.Inspections
{
    class EnumerationDeclaredWithinWorksheetInspectionTests : InspectionTestsBase
    {
        [Test]
        [Category("Inspections")]
        public void EnumerationDeclaredWithinWorksheet_InspectionName()
        {
            var inspection = new EnumerationDeclaredWithinWorksheetInspection(null, null);

            Assert.AreEqual(nameof(EnumerationDeclaredWithinWorksheetInspection), inspection.Name);
        }

        [Test]
        [Category("Inspections")]
        [Category("EnumerationDeclaredWithinWorksheet")]
        public void Project_with_multiple_enumerations_flags_only_enum_declared_within_worksheets()
        {
            #region InputCode
            const string worksheetCode = @"Option Explicit
Public Enum WorksheetEnum
    wsMember1 = 0
    wsMember1 = 1
End Enum
";
            const string standardModuleCode = @"Option Explicit
Public Enum StdModEnum
    stdMember1 = 0
    stdMember1 = 2
End Enum
";
            const string classModuleCode = @"Option Explicit
Public Enum ClassModEnum
    classMember1 = 0
    classMember2 = 3
End Enum
";
            const string userFormModuleCode = @"Option Explicit
Public Enum FormEnum
    formMember1 = 0
    formMember2 = 4
End Enum
";
            #endregion

            var inspectionResults = InspectionResultsForModules(
                ("Sheet", worksheetCode, ComponentType.DocObject),
                ("StdModule", standardModuleCode, ComponentType.StandardModule),
                ("ClsMod", classModuleCode, ComponentType.ClassModule),
                ("UserFormMod", userFormModuleCode, ComponentType.UserForm));

            int actual = inspectionResults.Count();

            Assert.AreEqual(1, actual);
        }

        [Test]
        [Category("Inspections")]
        [Category("EnumerationDeclaredWithinWorksheet")]
        [TestCase(ComponentType.ActiveXDesigner)]
        [TestCase(ComponentType.ClassModule)]
        [TestCase(ComponentType.ComComponent)]
        [TestCase(ComponentType.Document)]
        [TestCase(ComponentType.MDIForm)]
        [TestCase(ComponentType.PropPage)]
        [TestCase(ComponentType.RelatedDocument)]
        [TestCase(ComponentType.ResFile)]
        [TestCase(ComponentType.StandardModule)]
        [TestCase(ComponentType.Undefined)]
        [TestCase(ComponentType.UserControl)]
        [TestCase(ComponentType.UserForm)]
        [TestCase(ComponentType.VBForm)]
        public void Enumerations_declared_within_non_worksheet_object_have_no_inpsection_result(ComponentType componentType)
        {
            const string code = @"Option Explicit
Public Enum TestEnum
    Member1
    Member2
End Enum
";

            var inspectionResults = InspectionResultsForModules((componentType.ToString() + "module", code, componentType));
            Assert.IsFalse(inspectionResults.Any());
        }

        [Test]
        [Category("Inspections")]
        [Category("EnumerationDeclaredWithinWorksheet")]
        public void Private_type_declared_within_worksheet_has_no_inspection_result()
        {
            const string code = @"Option Explicit

Private Type THelper
    Name As String
    Address As String
End Type

Private this as THelper
";

            var inspectionResults = InspectionResultsForModules(("WorksheetForTest", code, ComponentType.DocObject));

            Assert.IsFalse(inspectionResults.Any());
        }

        protected override IInspection InspectionUnderTest(RubberduckParserState state)
        {
            return new EnumerationDeclaredWithinWorksheetInspection(state, state.ProjectsProvider);
        }
    }
}
