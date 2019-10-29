# CHANGELOG

## 1.0.0-beta.21 - 2019-10-29

Updated package description and documentation home page. No changes to the SDK
itself.

## 1.0.0-beta.20 - 2019-10-24

### Changed

- Eliminated the need to create a ProcessingContext, simplifying the API. In
  fact, the ProcessingContext class has been completely removed. All of its
  document processing methods have been moved up to the PrizmDocServerClient
  class. Now, you simply construct a single PrizmDocServerClient instance and
  use it directly to do document processing work.

### Removed

- ProcessingContext class.
- UploadAsync methods.

## 1.0.0-beta.19 - 2019-10-17

First public beta release.
