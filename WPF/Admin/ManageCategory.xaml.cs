﻿
using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using Service;

namespace WPF.Admin
{
   
    public partial class ManageCategory : Page
    {
        private CategoryService _categoryService;

        public ManageCategory()
        {
            _categoryService = new CategoryService();
            InitializeComponent();
        }
        private void loadData()
        {
            CategoryGrid.ItemsSource = _categoryService.getAll();

        }


        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryPanel.Visibility = Visibility.Visible;
            EditCategoryPanel.Visibility = Visibility.Collapsed;
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            Category category = (Category)CategoryGrid.SelectedItem;
            if (category == null)
            {
                MessageBox.Show("Hãy chọn loại để sửa !");
                return;
            }

            AddCategoryPanel.Visibility = Visibility.Collapsed;
            EditCategoryPanel.Visibility = Visibility.Visible;
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            Category category = (Category)CategoryGrid.SelectedItem;
            if (category != null)
            {
                                _categoryService.deleteCategorias(category);
                MessageBox.Show("Xóa thành công!");
                loadData();
            }
            else
            {
                MessageBox.Show("Hãy chọn loại để xóa!");
            }
        }

        private void clearForm()
        {
            txtCategoryName.Clear();
            txtDescription.Clear();
        }
        private void SaveEditCategory_Click(object sender, RoutedEventArgs e)
        {
            Category category = (Category)CategoryGrid.SelectedItem;
            if (category != null)
            {
                // Lấy giá trị từ input
                string categoryName = txtEditCategoryName.Text.Trim();
                string description = txtEditDescription.Text.Trim();

                if (string.IsNullOrEmpty(categoryName))
                {
                    MessageBox.Show("Tên thể loại không được để trống!");
                    return;
                }
                if (categoryName.Length > 50)
                {
                    MessageBox.Show("Tên thể loại không được dài quá 50 ký tự!");
                    return;
                }

                if (description.Length > 200)
                {
                    MessageBox.Show("Mô tả không được dài quá 200 ký tự!");
                    return;
                }

                category.CategoryName = categoryName;
                category.Description = description;

                if (_categoryService.updateCategorias(category))
                {
                    MessageBox.Show("Sửa thể loại thành công!");
                    loadData();
                    EditCategoryPanel.Visibility = Visibility.Collapsed;
                    clearForm();
                }
                else
                {
                    MessageBox.Show("Sửa thể loại thất bại!");
                }
            }
            else
            {
                MessageBox.Show("Hãy chọn loại để sửa!");
            }
        }

        private void CancelEditCategory_Click(object sender, RoutedEventArgs e)
        {
            EditCategoryPanel.Visibility = Visibility.Collapsed;
            CategoryGrid.SelectedItem = null;
        }

        private void CancelAddCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryPanel.Visibility = Visibility.Collapsed;
            clearForm();
        }

        private void SaveCategory_Click(object sender, RoutedEventArgs e)
        {
            // Lấy giá trị từ input và loại bỏ khoảng trắng thừa
            string categoryName = txtCategoryName.Text.Trim();
            string description = txtDescription.Text.Trim();

            // Validation cho CategoryName
            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Tên thể loại không được để trống!");
                return;
            }
            if (categoryName.Length > 50)
            {
                MessageBox.Show("Tên thể loại không được dài quá 50 ký tự!");
                return;
            }

            // Validation cho Description (tùy chọn)
            if (description.Length > 200)
            {
                MessageBox.Show("Mô tả không được dài quá 200 ký tự!");
                return;
            }

            // Tạo đối tượng Category và gán giá trị
            Category category = new Category();
            category.CategoryName = categoryName;
            category.Description = description;

            // Gọi service để thêm danh mục
            if (_categoryService.addCategorias(category))
            {
                MessageBox.Show("Thêm thể loại thành công!");
                loadData();
                AddCategoryPanel.Visibility = Visibility.Collapsed;
                clearForm();
            }
            else
            {
                MessageBox.Show("Thêm thể loại thất bại!");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadData();
        }
    }

}
