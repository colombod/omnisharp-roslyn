using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using OmniSharp.Models;

namespace OmniSharp
{
    [Export(typeof(RequestHandler<Request, IEnumerable<QuickFix>>))]
    public class MembersAsFlatService : RequestHandler<Request, IEnumerable<QuickFix>>
    {
        private readonly OmnisharpWorkspace _workspace;

        [ImportingConstructor]
        public MembersAsFlatService(OmnisharpWorkspace workspace)
        {
            _workspace = workspace;
        }

        public async Task<IEnumerable<QuickFix>> Handle(Request request)
        {
            var stack = new List<FileMemberElement>(await StructureComputer.Compute(_workspace.GetDocuments(request.FileName)));
            var ret = new List<QuickFix>();
            while (stack.Count > 0)
            {
                var node = stack[0];
                stack.Remove(node);
                ret.Add(node.Location);
                stack.AddRange(node.ChildNodes);
            }
            return ret;
        }
    }
}