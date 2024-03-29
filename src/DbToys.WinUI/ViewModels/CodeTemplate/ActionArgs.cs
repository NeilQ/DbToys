﻿using Windows.Storage;

namespace DbToys.ViewModels.CodeTemplate;

public record RenamedArgs(string OldName, string NewName, string OldPath, string NewPath);

public record ProjectDeletedArg(string FolderName, string FolderPath);

public record ProjectCreatedArg(StorageFolder Folder);

public record TemplateCreatedArg(StorageFile File);

public record TemplateDeletedArg(string FileName, string FilePath, string FolderName, string FolderPath);

