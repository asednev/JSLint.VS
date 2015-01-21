using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace JSLint.VS2010.VS2010
{
    internal static class EnvDteExtensions
    {
        ///<returns>All actual projects in the solution. Excludes solution folders...</returns>
        public static IEnumerable<Project> AllProjects(this Solution me)
        {
            return me.Projects.Cast<Project>()
                .SelectMany(ProjectOrSolutionFolderProjects)
                .ToArray();
        }

        ///<returns>The project if it really is a project. All contained projects, recursively, if it is a solution folder.</returns>
        public static IEnumerable<Project> ProjectOrSolutionFolderProjects(this Project proj)
        {
            if (proj.Kind != ProjectKinds.vsProjectKindSolutionFolder)
            {
                return new[] { proj };
            }

            return proj.SubProjects().SelectMany(ProjectOrSolutionFolderProjects).ToArray();
        }    

        /// <summary>Recursively fetches the projects contained in a solution folder</summary>
        private static IEnumerable<Project> SubProjects(this Project me)
        {
            return me.ProjectItems.Cast<ProjectItem>()
                .Select(proj => proj.SubProject)
                .Where(sub => sub != null)
                .SelectMany(ProjectOrSolutionFolderProjects)
                .ToArray();
        }    
    }
}