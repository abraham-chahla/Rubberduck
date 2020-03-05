﻿using Antlr4.Runtime.Misc;
using Rubberduck.Inspections.Abstract;
using Rubberduck.Parsing.Common;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Resources.Inspections;
using Rubberduck.Parsing.VBA;
using Rubberduck.Resources.Experimentals;
using Rubberduck.Parsing;

namespace Rubberduck.Inspections.Concrete
{
    /// <summary>
    /// Identifies empty 'Do...Loop While' blocks that can be safely removed.
    /// </summary>
    /// <why>
    /// Dead code should be removed. A loop without a body is usually redundant.
    /// </why>
    /// <example hasResults="true">
    /// <![CDATA[
    /// Public Sub DoSomething(ByVal foo As Long)
    ///     Do
    ///         ' no executable statement...
    ///     Loop While foo < 100
    /// End Sub
    /// ]]>
    /// </example>
    /// <example hasResults="false">
    /// <![CDATA[
    /// Public Sub DoSomething(ByVal foo As Long)
    ///     Do
    ///         Debug.Print foo
    ///     Loop While foo < 100
    /// End Sub
    /// ]]>
    /// </example>
    [Experimental(nameof(ExperimentalNames.EmptyBlockInspections))]
    internal sealed class EmptyDoWhileBlockInspection : ParseTreeInspectionBase<VBAParser.DoLoopStmtContext>
    {
        public EmptyDoWhileBlockInspection(IDeclarationFinderProvider declarationFinderProvider)
            : base(declarationFinderProvider)
        {
            ContextListener = new EmptyDoWhileBlockListener();
        }

        protected override IInspectionListener<VBAParser.DoLoopStmtContext> ContextListener { get; }

        protected override string ResultDescription(QualifiedContext<VBAParser.DoLoopStmtContext> context)
        {
            return InspectionResults.EmptyDoWhileBlockInspection;
        }

        public class EmptyDoWhileBlockListener : EmptyBlockInspectionListenerBase<VBAParser.DoLoopStmtContext>
        {
            public override void EnterDoLoopStmt([NotNull] VBAParser.DoLoopStmtContext context)
            {
                InspectBlockForExecutableStatements(context.block(), context);
            }
        }
    }
}
