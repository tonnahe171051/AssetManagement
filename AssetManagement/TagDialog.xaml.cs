using AssetManagement.DataAccess;
using AssetManagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AssetManagement
{
    /// <summary>
    /// Interaction logic for TagDialog.xaml
    /// </summary>
    public partial class TagDialog : Window
    {
        private readonly ITagRepository _tagRepository;
        private Tag _currentTag;
        private bool _isEditMode;
        public TagDialog(ITagRepository tagRepository, Tag tag = null)
        {
            InitializeComponent();
            _tagRepository = tagRepository;
            _currentTag = tag;
            _isEditMode = tag != null;

            if (_isEditMode)
            {
                Title = "Edit Tag";
                txtTagName.Text = _currentTag.Name;
                btnSave.Content = "Update";
            }
        }



        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tagName = txtTagName.Text.Trim();

                if (string.IsNullOrWhiteSpace(tagName))
                {
                    MessageBox.Show("Tag name is required", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra tag trùng
                if (await _tagRepository.TagExistsAsync(tagName, _isEditMode ? _currentTag.Id : null))
                {
                    MessageBox.Show("A tag with this name already exists", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_isEditMode)
                {
                    _currentTag.Name = tagName;
                    await _tagRepository.UpdateTagAsync(_currentTag);
                }
                else
                {
                    var newTag = new Tag { Name = tagName };
                    await _tagRepository.AddTagAsync(newTag);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving tag: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
