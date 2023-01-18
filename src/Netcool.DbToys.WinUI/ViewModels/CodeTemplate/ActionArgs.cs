using Windows.Storage;

namespace Netcool.DbToys.WinUI.ViewModels.CodeTemplate;

public record RenamedArgs(string OldName, string NewName, string OldPath, string NewPath);

public record ProjectDeletedArg(string FolderName, string FolderPath);

public record TemplateCreatedArg(StorageFile File);

public record TemplateDeletedArg(string FileName, string FilePath, string FolderName, string FolderPath);

