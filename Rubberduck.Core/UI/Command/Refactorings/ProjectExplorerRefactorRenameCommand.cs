using System.Linq;
using System.Runtime.InteropServices;
using Rubberduck.Interaction;
using Rubberduck.Parsing.Rewriter;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;
using Rubberduck.Refactorings;
using Rubberduck.Refactorings.Rename;
using Rubberduck.UI.Refactorings.Rename;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;

namespace Rubberduck.UI.Command.Refactorings
{
    [ComVisible(false)]
    public class ProjectExplorerRefactorRenameCommand : RefactorCommandBase
    {
        private readonly RubberduckParserState _state;
        private readonly IRewritingManager _rewritingManager;
        private readonly IMessageBox _msgBox;
        private readonly IRefactoringPresenterFactory _factory;

        public ProjectExplorerRefactorRenameCommand(IVBE vbe, RubberduckParserState state, IMessageBox msgBox, IRefactoringPresenterFactory factory, IRewritingManager rewritingManager)
            : base(vbe)
        {
            _state = state;
            _rewritingManager = rewritingManager;
            _msgBox = msgBox;
            _factory = factory;
        }

        protected override bool EvaluateCanExecute(object parameter)
        {
            return _state.Status == ParserState.Ready;
        }

        protected override void OnExecute(object parameter)
        {
            var refactoring = new RenameRefactoring(Vbe, _factory, _msgBox, _state, _state.ProjectsProvider, _rewritingManager);
            var target = GetTarget();
            if (target != null)
            {
                refactoring.Refactor(target);
            }
        }

        private Declaration GetTarget()
        {
            string selectedComponentName;
            using (var selectedComponent = Vbe.SelectedVBComponent)
            {
                selectedComponentName = selectedComponent?.Name;
            }

            string activeProjectId;
            using (var activeProject = Vbe.ActiveVBProject)
            {
                activeProjectId = activeProject?.ProjectId;
            }

            if (activeProjectId == null)
            {
                return null;
            }

            if (selectedComponentName == null)
            {
                return _state.DeclarationFinder.UserDeclarations(DeclarationType.Project)
                    .SingleOrDefault(d => d.ProjectId == activeProjectId);
            }

            return _state.DeclarationFinder.UserDeclarations(DeclarationType.Module)
                .SingleOrDefault(t => t.IdentifierName == selectedComponentName
                                      && t.ProjectId == activeProjectId);
        }
    }
}
