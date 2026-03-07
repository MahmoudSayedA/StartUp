namespace Application.Common.Exceptions
{
    public class PathNotFoundException : AppException
    {
        public PathNotFoundException(string fullPath)
        : base($"Couldn't Find Directories in this path: {fullPath}\n check the correct path.", 500, null)
        {
        }
    }
}
