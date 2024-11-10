using AssetManagement.DataAccess;
using AssetManagement.Repository;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AssetManagement
{
    public partial class MainWindow : Window
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IFileRepository _fileRepository;
        private readonly ITagRepository _tagRepository;
        private ObservableCollection<Asset> _assets;
        private ObservableCollection<Tag> _tags;
        private Asset _selectedAsset;
        private bool _isLoading = false;
        private DataAccess.File _pendingFile;

        public MainWindow(IAssetRepository assetRepository, IFileRepository fileRepository, ITagRepository tagRepository)
        {
            InitializeComponent();
            _assetRepository = assetRepository;
            _fileRepository = fileRepository;
            _tagRepository = tagRepository;
            LoadInitialDataAsync();
        }

        private async void LoadInitialDataAsync()
        {
            await LoadTagsAsync();
            await LoadAssetsAsync();
        }

        private async Task LoadAssetsAsync()
        {
            if (_isLoading) return;

            _isLoading = true;
            try
            {
                var assets = await _assetRepository.GetAllAssetsAsync();
                _assets = new ObservableCollection<Asset>(assets);
                lbAssets.ItemsSource = _assets;
                UpdateStatusBar("Assets loaded successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading assets: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatusBar("Error loading assets");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void UpdateStatusBar(string message)
        {
            txtStatus.Text = message;
        }

        private void ClearAssetDetails()
        {
            txtAssetName.Clear();
            txtAssetDescription.Clear();
            dgFiles.ItemsSource = null;
        }

        private async void btnNewAsset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtAssetName.Text))
                {
                    MessageBox.Show("Asset name is required", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newAsset = new Asset
                {
                    Name = txtAssetName.Text.Trim(),
                    Description = txtAssetDescription.Text?.Trim()
                };

                await _assetRepository.AddAssetAsync(newAsset);
                _assets.Add(newAsset);
                UpdateStatusBar("New asset created successfully");
                ClearAssetDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating asset: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatusBar("Error creating asset");
            }
        }

        private async void lbAssets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAsset = lbAssets.SelectedItem as Asset;
            if (selectedAsset != null)
            {
                try
                {
                    _selectedAsset = await _assetRepository.GetAssetByIdAsync(selectedAsset.Id);
                    txtAssetName.Text = _selectedAsset.Name;
                    txtAssetDescription.Text = _selectedAsset.Description;
                    dgFiles.ItemsSource = _selectedAsset.Files;
                    UpdateStatusBar($"Selected asset: {_selectedAsset.Name}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading asset details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateStatusBar("Error loading asset details");
                }
            }
        }

        private async void btnSaveAsset_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAsset == null)
            {
                MessageBox.Show("No asset selected", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(txtAssetName.Text))
                {
                    MessageBox.Show("Asset name is required", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _selectedAsset.Name = txtAssetName.Text.Trim();
                _selectedAsset.Description = txtAssetDescription.Text?.Trim();

                await _assetRepository.UpdateAssetAsync(_selectedAsset);

                // Cập nhật item trong ObservableCollection
                var index = _assets.IndexOf(_selectedAsset);
                if (index != -1)
                {
                    _assets[index] = _selectedAsset;
                }

                UpdateStatusBar("Asset updated successfully");
                await LoadAssetsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving asset: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatusBar("Error saving asset");
            }
        }

        private async void btnDeleteAsset_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAsset == null)
            {
                MessageBox.Show("Please select an asset to delete", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this asset?", "Confirm Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _assetRepository.DeleteAssetAsync(_selectedAsset.Id);
                    _assets.Remove(_selectedAsset);
                    ClearAssetDetails();
                    UpdateStatusBar("Asset deleted successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting asset: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateStatusBar("Error deleting asset");
                }
            }
        }

        private async void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAsset == null)
            {
                MessageBox.Show("Please select an asset first", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var openFileDialog = new OpenFileDialog
            {
                Title = "Select File to Upload",
                Filter = "All Files (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var sourceFilePath = openFileDialog.FileName;

                    // Check if file size is within reasonable limits (e.g., 100MB)
                    var fileInfo = new FileInfo(sourceFilePath);
                    if (fileInfo.Length > 100 * 1024 * 1024) // 100MB
                    {
                        MessageBox.Show("File size exceeds 100MB limit", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _pendingFile = await _fileRepository.UploadFileAsync(sourceFilePath, _selectedAsset.Id);

                    // Hiển thị file lên DataGrid
                    var files = await _fileRepository.GetFilesByAssetIdAsync(_selectedAsset.Id);
                    files.Add(_pendingFile); // Thêm file mới upload
                    dgFiles.ItemsSource = files;

                    UpdateStatusBar($"File '{_pendingFile.Name}' uploaded. Please select a tag and save.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error uploading file: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateStatusBar("Error uploading file");
                }
            }
        }

        private async Task LoadTagsAsync()
        {
            if (_isLoading) return;

            _isLoading = true;
            try
            {
                var tags = await _tagRepository.GetAllTagsAsync();
                _tags = new ObservableCollection<Tag>(tags);
                lbTags.ItemsSource = _tags;
                UpdateStatusBar("Tags loaded successfully");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tags: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { _isLoading = false; }
        }

        private async void btnNewTag_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TagDialog(_tagRepository);
            if (dialog.ShowDialog() == true)
            {
                await LoadTagsAsync();
                UpdateStatusBar("Tag created successfully");
            }
        }

        private async void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (_pendingFile == null)
            {
                MessageBox.Show("No pending file to save", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_pendingFile.TagId == null)
            {
                MessageBox.Show("Please select a tag for the file first", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Lưu file vào database
                await _fileRepository.AddFileAsync(_pendingFile);

                // Refresh DataGrid
                var files = await _fileRepository.GetFilesByAssetIdAsync(_selectedAsset.Id);
                dgFiles.ItemsSource = files;

                _pendingFile = null; // Reset pending file
                UpdateStatusBar("File saved successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatusBar("Error saving file");
            }
        }

        private void lbTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTag = lbTags.SelectedItem as Tag;
            if (selectedTag != null && _pendingFile != null)
            {
                _pendingFile.TagId = selectedTag.Id;
                _pendingFile.Tag = selectedTag;
                dgFiles.Items.Refresh();
                UpdateStatusBar($"Tag '{selectedTag.Name}' assigned to file. Click Save File to complete.");
            }
        }

        private async void btnDeleteFile_Click(object sender, RoutedEventArgs e)
        {
            var selectedFile = dgFiles.SelectedItem as DataAccess.File;
            if (selectedFile == null)
            {
                MessageBox.Show("Please select a file to delete", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this file?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _fileRepository.DeleteFileAsync(selectedFile.Id);

                    if (selectedFile == _pendingFile)
                    {
                        _pendingFile = null;
                    }

                    // Refresh DataGrid
                    var files = await _fileRepository.GetFilesByAssetIdAsync(_selectedAsset.Id);
                    dgFiles.ItemsSource = files;

                    UpdateStatusBar("File deleted successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting file: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateStatusBar("Error deleting file");
                }
            }
        }

        private void dgFiles_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedFile = dgFiles.SelectedItem as DataAccess.File;
            if (selectedFile != null)
            {
                try
                {
                    // Mở file bằng ứng dụng mặc định
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = selectedFile.Path,
                        UseShellExecute = true
                    });
                    UpdateStatusBar($"Opened: {selectedFile.Name}");
                }
                catch (Exception ex)
                {
                    UpdateStatusBar($"Error opening file: {ex.Message}");
                    MessageBox.Show(
                        $"Error opening file: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private async void btnDeleteTag_Click(object sender, RoutedEventArgs e)
        {
            var selectedTag = lbTags.SelectedItem as Tag;
            if (selectedTag == null)
            {
                MessageBox.Show("Please select a tag to delete", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to delete this tag?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _tagRepository.DeleteTagAsync(selectedTag.Id);
                    await LoadTagsAsync();
                    UpdateStatusBar("Tag deleted successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting tag: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void btnEditTag_Click(object sender, RoutedEventArgs e)
        {
            var selectedTag = lbTags.SelectedItem as Tag;
            if (selectedTag == null)
            {
                MessageBox.Show("Please select a tag to edit", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new TagDialog(_tagRepository, selectedTag);
            if (dialog.ShowDialog() == true)
            {
                await LoadTagsAsync();
                UpdateStatusBar("Tag updated successfully");
            }
        }

        private async void txtSearchFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedAsset == null) return;

            string searchTerm = txtSearchFile.Text.Trim();
            try
            {
                var files = await _fileRepository.GetFilesByNameAsync(_selectedAsset.Id, searchTerm);
                dgFiles.ItemsSource = files;
                UpdateStatusBar($"Found {files.Count} files for search term '{searchTerm}'");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatusBar("Error searching files");
            }
        }
    }
}