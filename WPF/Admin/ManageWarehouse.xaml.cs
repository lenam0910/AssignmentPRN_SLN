﻿using System;
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
    /// Interaction logic for ManageWarehouse.xaml
    /// </summary>
    public partial class ManageWarehouse : Page
    {
        private SupplierService supplierService;
        private WarehousesService warehousesService;
        public ManageWarehouse()
        {
            warehousesService = new WarehousesService();
            supplierService = new SupplierService();
            InitializeComponent();
        }

        public void load()
        {
            var listSup = supplierService.GetAllSuppliers();
            SupplierComboBox.ItemsSource = listSup;
            SupplierComboBox.DisplayMemberPath = "SupplierName";
            SupplierComboBox.SelectedValuePath = "SupplierId";


        }
        public void load2(DataAccess.Models.Supplier supplier)
        {
            var listWare = warehousesService.GetAllWarehousesByIdSupplier(supplier.SupplierId);
            WarehouseGrid.ItemsSource = listWare;

        }

        private void clear()
        {
            txtWarehouseName.Clear();
            txtLocation.Clear();
            txtCapacity.Clear();
        }
        private void OpenAddWarehousePopup(object sender, RoutedEventArgs e)
        {
            if (SupplierComboBox.SelectedItem != null)
            {
                AddWarehousePanel.Visibility = Visibility.Visible;
                EditWarehousePanel.Visibility = Visibility.Collapsed;
                WarehouseGrid.SelectedItem = null;
                clear();
            }
            else
            {
                MessageBox.Show("Hãy chọn nhà cung cấp sau đó chọn kho hàng để thêm!");

            }

        }

        private void OpenEditWarehousePopup(object sender, RoutedEventArgs e)
        {
            if (SupplierComboBox.SelectedItem != null)
            {

                if (WarehouseGrid.SelectedItem != null)
                {
                    EditWarehousePanel.Visibility = Visibility.Visible;
                    Warehouse warehouse = WarehouseGrid.SelectedItem as Warehouse;
                    txtEditWarehouseName.Text = warehouse.WarehouseName;
                    txtEditLocation.Text = warehouse.Location;
                    txtEditCapacity.Text = warehouse.Capacity.ToString();
                }
                else
                {
                    MessageBox.Show("Hãy chọn kho hàng trước khi sửa!");
                }
            }
            else
            {
                MessageBox.Show("Hãy chọn nhà cung cấp sau đó chọn kho hàng để sửa!");
            }
        }

        private void DeleteWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierComboBox.SelectedItem != null)
            {
                DataAccess.Models.Supplier supplier = (DataAccess.Models.Supplier)SupplierComboBox.SelectedItem;

                if (WarehouseGrid.SelectedItem != null)
                {
                    Warehouse warehouse = WarehouseGrid.SelectedItem as Warehouse;
                    warehousesService.DeleteWarehouses(warehouse);
                    MessageBox.Show("Xoá kho hàng thành công!");
                    load2(supplier);

                }
                else
                {
                    MessageBox.Show("Hãy chọn kho hàng trước khi xoá!");
                }
            }
            else
            {
                SupplierComboBox.Focus();
                MessageBox.Show("Hãy chọn nhà cung cấp sau đó chọn kho hàng để xóa!");
            }
        }



        private void SupplierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataAccess.Models.Supplier supplier = (DataAccess.Models.Supplier)SupplierComboBox.SelectedItem;
            var listWare = warehousesService.GetAllWarehousesByIdSupplier(supplier.SupplierId);
            WarehouseGrid.ItemsSource = listWare;
        }

        private void SaveWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierComboBox.SelectedItem != null)
            {

                DataAccess.Models.Supplier supplier = (DataAccess.Models.Supplier)SupplierComboBox.SelectedItem;
                Warehouse ware = new Warehouse();
                ware.WarehouseName = txtWarehouseName.Text;
                ware.Location = txtLocation.Text;
                ware.SupplierId = supplier.SupplierId;
                if (int.TryParse(txtCapacity.Text, out int capacity))
                {
                    ware.Capacity = capacity;
                }
                else
                {
                    MessageBox.Show("Số lượng phải nhập số > 0");
                    return;
                }
                if (warehousesService.AddWarehouses(ware))
                {
                    MessageBox.Show("Thêm kho hàng thành công!");
                    AddWarehousePanel.Visibility = Visibility.Collapsed;
                    load2(supplier);

                }
                else
                {
                    MessageBox.Show("Thêm kho hàng thất bại!");
                }
            }
            else
            {
                SupplierComboBox.Focus();
                MessageBox.Show("Hãy chọn nhà cung cấp sau đó chọn kho hàng để thêm!");
            }
        }

        private void CloseAddWarehousePopup(object sender, RoutedEventArgs e)
        {
            AddWarehousePanel.Visibility = Visibility.Collapsed;
            clear();
        }

        private void SaveEditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierComboBox.SelectedItem != null)
            {
                DataAccess.Models.Supplier supplier = (DataAccess.Models.Supplier)SupplierComboBox.SelectedItem;
                if (WarehouseGrid.SelectedItem != null)
                {
                    Warehouse warehouse = WarehouseGrid.SelectedItem as Warehouse;
                    warehouse.WarehouseName = txtEditWarehouseName.Text;
                    warehouse.Location = txtEditLocation.Text;
                    if (int.TryParse(txtEditCapacity.Text, out int capacity))
                    {
                        warehouse.Capacity = capacity;
                    }
                    else
                    {
                        MessageBox.Show("Số lượng phải nhập số > 0");
                        return;
                    }
                    if (warehousesService.UpdateWarehouses(warehouse))
                    {
                        MessageBox.Show("Sửa kho hàng thành công!");
                        EditWarehousePanel.Visibility = Visibility.Collapsed;
                        load2(supplier);
                        WarehouseGrid.UnselectAll();
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn kho hàng trước khi sửa!");
                }
            }
            else
            {
                SupplierComboBox.Focus();
                MessageBox.Show("Hãy chọn nhà cung cấp sau đó chọn kho hàng để sửa!");
            }
        }

        private void CloseEditWarehousePopup(object sender, RoutedEventArgs e)
        {
            EditWarehousePanel.Visibility = Visibility.Collapsed;
            WarehouseGrid.UnselectAll();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }
    }
}
