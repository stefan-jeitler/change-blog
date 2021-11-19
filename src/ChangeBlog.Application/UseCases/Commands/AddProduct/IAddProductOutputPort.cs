using System;
using System.Collections.Generic;
using ChangeBlog.Application.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.AddProduct;

public interface IAddProductOutputPort
{
    void AccountDoesNotExist(Guid accountId);
    void AccountDeleted(Guid accountId);
    void InvalidName(string name);
    void ProductAlreadyExists(Guid productId);
    void VersioningSchemeDoesNotExist(Guid versioningSchemeId);
    void Conflict(Conflict conflict);
    void Created(Guid accountId, Guid productId);
    void NotSupportedLanguageCode(string languageCode, IList<string> supportedLangCodes);
}