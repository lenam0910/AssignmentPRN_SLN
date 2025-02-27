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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataAccess.Models;
using Service;

namespace WPF.Admin
{
    /// <summary>
    /// Interaction logic for ManageCategory.xaml
    /// </summary>
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
                category.CategoryName = txtEditCategoryName.Text;
                category.Description = txtEditDescription.Text;
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
                MessageBox.Show("Hãy chọn loại để sửa !");

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
            Category category = new Category();
            category.CategoryName = txtCategoryName.Text;
            category.Description = txtDescription.Text;
            if (_categoryService.addCategorias(category))
            {
                MessageBox.Show("Thêm thể loại thành công!");
                loadData();
                AddCategoryPanel.Visibility = Visibility.Collapsed;

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
