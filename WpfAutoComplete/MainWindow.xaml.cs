using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace WpfAutoComplete
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public List<ItemComboInfo> ListaInicial = new List<ItemComboInfo>();

        private List<ItemComboInfo> _lista = new List<ItemComboInfo>();
        public List<ItemComboInfo> Lista
        {
            get { return _lista; }
            set { _lista = value; OnPropertyChanged("Lista"); }
        }

        private List<ItemComboInfo> _lista2 = new List<ItemComboInfo>();
        public List<ItemComboInfo> Lista2
        {
            get { return _lista2; }
            set { _lista2 = value; OnPropertyChanged("Lista2"); }
        }

        private List<ItemComboInfo> _lista3 = new List<ItemComboInfo>();
        public List<ItemComboInfo> Lista3
        {
            get { return _lista3; }
            set { _lista3 = value; OnPropertyChanged("Lista3"); }
        }

        private List<ItemComboInfo> _lista4 = new List<ItemComboInfo>();
        public List<ItemComboInfo> Lista4
        {
            get { return _lista4; }
            set { _lista4 = value; OnPropertyChanged("Lista4"); }
        }

        private List<ItemComboInfo> _lista5 = new List<ItemComboInfo>();
        public List<ItemComboInfo> Lista5
        {
            get { return _lista5; }
            set { _lista5 = value; OnPropertyChanged("Lista5"); }
        }

        private List<ItemComboInfo> _lista6 = new List<ItemComboInfo>();
        public List<ItemComboInfo> Lista6
        {
            get { return _lista6; }
            set { _lista6 = value; OnPropertyChanged("Lista6"); }
        }


        private List<ItemComboInfo> _lista7 = new List<ItemComboInfo>();
        public List<ItemComboInfo> Lista7
        {
            get { return _lista7; }
            set { _lista7 = value; OnPropertyChanged("Lista7"); }
        }

        private List<ItemComboInfo> _lista8 = new List<ItemComboInfo>();
        public List<ItemComboInfo> Lista8
        {
            get { return _lista8; }
            set { _lista8 = value; OnPropertyChanged("Lista8"); }
        }

        private List<ItemComboInfo> _lista9 = new List<ItemComboInfo>();
        public List<ItemComboInfo> Lista9
        {
            get { return _lista9; }
            set { _lista9 = value; OnPropertyChanged("Lista9"); }
        }

        private List<ItemComboInfo> _lista10 = new List<ItemComboInfo>();
        public List<ItemComboInfo> Lista10
        {
            get { return _lista10; }
            set { _lista10 = value; OnPropertyChanged("Lista10"); }
        }

        private string _link = "";
        public string Link
        {
            get { return _link; }
            set { _link = value; OnPropertyChanged("Link"); }
        }

        private Brush _colorLink = null;
        public Brush ColorLink
        {
            get { return _colorLink; }
            set { _colorLink = value; OnPropertyChanged("ColorLink"); }
        }

        private ItemComboInfo _itemSelected = null;
        public ItemComboInfo ItemSelected
        {
            get { return _itemSelected; }
            set { _itemSelected = value; OnPropertyChanged("ItemSelected"); }
        }

        private ItemComboInfo _itemSelected3 = null;
        public ItemComboInfo ItemSelected3
        {
            get { return _itemSelected3; }
            set { _itemSelected3 = value; OnPropertyChanged("ItemSelected3"); }
        }

        private string _selectedValue = null;
        public string SelectedValue
        {
            get { return _selectedValue; }
            set { _selectedValue = value; OnPropertyChanged("SelectedValue"); }
        }

        private string _selectedValue4 = null;
        public string SelectedValue4
        {
            get { return _selectedValue4; }
            set { _selectedValue4 = value; OnPropertyChanged("SelectedValue4"); }
        }

        private Nullable<int> _selectedIndex = null;
        public Nullable<int> SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; OnPropertyChanged("SelectedIndex"); }
        }

        private Nullable<int> _selectedIndex5 = null;
        public Nullable<int> SelectedIndex5
        {
            get { return _selectedIndex5; }
            set { _selectedIndex5 = value; OnPropertyChanged("SelectedIndex5"); }
        }

        private bool _hasItems;
        public bool HasItems
        {
            get { return _hasItems; }
            set { _hasItems = value; OnPropertyChanged("HasItems"); }
        }

        private bool _hasItems6;
        public bool HasItems6
        {
            get { return _hasItems6; }
            set { _hasItems6 = value; OnPropertyChanged("HasItems6"); }
        }

        private bool _isEnabled7 = true;
        public bool IsEnabled7
        {
            get { return _isEnabled7; }
            set { _isEnabled7 = value; OnPropertyChanged("IsEnabled7"); }
        }

        private Visibility _visibility8;
        public Visibility Visibility8
        {
            get { return _visibility8; }
            set { _visibility8 = value; OnPropertyChanged("Visibility8"); }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Link = "Clique com bind";

            this.LocationChanged += new EventHandler((sender, e) => {
                this.autoComplete.FecharPopup();
            });

            List<ItemComboInfo> lista = new List<ItemComboInfo>();

            //Dictionary<string, string> mock = new Dictionary<string, string>();
            lista.Add(new ItemComboInfo() { Codigo = "A1", Descricao = "Aline" }); //1
            lista.Add(new ItemComboInfo() { Codigo = "A2", Descricao = "ANDRÉ" });
            lista.Add(new ItemComboInfo() { Codigo = "A3", Descricao = "ABDIEL" });
            lista.Add(new ItemComboInfo() { Codigo = "A4", Descricao = "ANA MARIA" });
            lista.Add(new ItemComboInfo() { Codigo = "A5", Descricao = "ARIANE" });

            lista.Add(new ItemComboInfo() { Codigo = "B1", Descricao = "BRUNO 01" }); // 6
            lista.Add(new ItemComboInfo() { Codigo = "B2", Descricao = "BRUNO 02" });
            lista.Add(new ItemComboInfo() { Codigo = "B3", Descricao = "BRUNO 03" });
            lista.Add(new ItemComboInfo() { Codigo = "B4", Descricao = "BRUNO 04" });
            lista.Add(new ItemComboInfo() { Codigo = "B5", Descricao = "BRUNO 05" });
            lista.Add(new ItemComboInfo() { Codigo = "B6", Descricao = "BRUNO 06" });
            lista.Add(new ItemComboInfo() { Codigo = "B7", Descricao = "BRUNO 07" });

            lista.Add(new ItemComboInfo() { Codigo = "C1", Descricao = "CARLOS" }); //13
            lista.Add(new ItemComboInfo() { Codigo = "C2", Descricao = "CATARINA" });
            lista.Add(new ItemComboInfo() { Codigo = "C3", Descricao = "CEZAR" });
            lista.Add(new ItemComboInfo() { Codigo = "C4", Descricao = "CINTHIA" });
            lista.Add(new ItemComboInfo() { Codigo = "C5", Descricao = "CELINE" });
            lista.Add(new ItemComboInfo() { Codigo = "C6", Descricao = "CELIA" });

            lista.Add(new ItemComboInfo() { Codigo = "D1", Descricao = "DARIO" }); //18
            lista.Add(new ItemComboInfo() { Codigo = "D2", Descricao = "DOUGLAS" });
            lista.Add(new ItemComboInfo() { Codigo = "D3", Descricao = "DIEGO" });
            lista.Add(new ItemComboInfo() { Codigo = "D4", Descricao = "DI MARIA" });
            lista.Add(new ItemComboInfo() { Codigo = "D5", Descricao = "DARLENE" });

            lista.Add(new ItemComboInfo() { Codigo = "E1", Descricao = "EDUARDO" }); //23
            lista.Add(new ItemComboInfo() { Codigo = "E2", Descricao = "EDUARDA" });
            lista.Add(new ItemComboInfo() { Codigo = "E3", Descricao = "ELIANA" });
            lista.Add(new ItemComboInfo() { Codigo = "E4", Descricao = "ELIZA" });
            lista.Add(new ItemComboInfo() { Codigo = "E5", Descricao = "ÉDISON" });

            lista.Add(new ItemComboInfo() { Codigo = "F1", Descricao = "FABRICIO" }); //28
            lista.Add(new ItemComboInfo() { Codigo = "F2", Descricao = "FELIPE" });
            lista.Add(new ItemComboInfo() { Codigo = "F3", Descricao = "FRANCISCO" });
            lista.Add(new ItemComboInfo() { Codigo = "F4", Descricao = "FABIOLA" });
            lista.Add(new ItemComboInfo() { Codigo = "F5", Descricao = "FERNANDO" });

            lista.Add(new ItemComboInfo() { Codigo = "G1", Descricao = "GABRIEL" }); //33
            lista.Add(new ItemComboInfo() { Codigo = "G2", Descricao = "GABRIELA" });
            lista.Add(new ItemComboInfo() { Codigo = "G3", Descricao = "GILDA" });
            lista.Add(new ItemComboInfo() { Codigo = "G4", Descricao = "GUSTAVO" });
            lista.Add(new ItemComboInfo() { Codigo = "G5", Descricao = "GOMES" });


            lista.Add(new ItemComboInfo() { Codigo = "J1", Descricao = "JOÃO" }); //38
            lista.Add(new ItemComboInfo() { Codigo = "J2", Descricao = "JUNIOR" });
            lista.Add(new ItemComboInfo() { Codigo = "J3", Descricao = "JOANAJOANAJOANAJOOANAJOANAJOANAJOANAOANAJOAOANAJOAOANAJOAOANAJOA" });
            lista.Add(new ItemComboInfo() { Codigo = "J4", Descricao = "JUVENAL" });
            lista.Add(new ItemComboInfo() { Codigo = "J5", Descricao = "JORGE" });

            lista.Add(new ItemComboInfo() { Codigo = "L1", Descricao = "LUANA" }); //43
            lista.Add(new ItemComboInfo() { Codigo = "L2", Descricao = "LUCAS" });
            lista.Add(new ItemComboInfo() { Codigo = "L3", Descricao = "LILO" });
            lista.Add(new ItemComboInfo() { Codigo = "L4", Descricao = "LILIANA" });
            lista.Add(new ItemComboInfo() { Codigo = "L5", Descricao = "LAURA" });

            lista.Add(new ItemComboInfo() { Codigo = "M1", Descricao = "MAURA" }); //48
            lista.Add(new ItemComboInfo() { Codigo = "M2", Descricao = "MARIA" });
            lista.Add(new ItemComboInfo() { Codigo = "M3", Descricao = "MARLENE" });

            lista.Add(new ItemComboInfo() { Codigo = "N1", Descricao = "NAIR" }); //53
            lista.Add(new ItemComboInfo() { Codigo = "N1", Descricao = "NAIR  SILVA TOLEDO" });
            lista.Add(new ItemComboInfo() { Codigo = "N2", Descricao = "NAIARA/SP" });
            lista.Add(new ItemComboInfo() { Codigo = "N2", Descricao = "NAIARA / RJ" });
            lista.Add(new ItemComboInfo() { Codigo = "N2", Descricao = "NAIARA" });
            lista.Add(new ItemComboInfo() { Codigo = "N3", Descricao = "NAIARO" });
            lista.Add(new ItemComboInfo() { Codigo = "N4", Descricao = "NILO" });
            lista.Add(new ItemComboInfo() { Codigo = "N5", Descricao = "Nômade " });
            lista.Add(new ItemComboInfo() { Codigo = "N6", Descricao = "NAIR Pereira" });
            lista.Add(new ItemComboInfo() { Codigo = "N7", Descricao = "NAIR Procópio" });
            lista.Add(new ItemComboInfo() { Codigo = "N8", Descricao = "NAIR Procópio Silva" });
            lista.Add(new ItemComboInfo() { Codigo = "N9", Descricao = "Nelma" });
            lista.Add(new ItemComboInfo() { Codigo = "N10", Descricao = "Núncio Asdrúbal Souza e Silva da Silva Sousa" }); //62

            Lista = lista;
            Lista2 = lista;
            Lista3 = lista;
            Lista4 = lista;
            Lista5 = lista;
            Lista6 = lista;
            Lista7 = lista;
            Lista8 = lista;
            Lista9 = lista;
            Lista10 = lista;
            ListaInicial = lista;
            //ColorLink = Brushes.Red;

        }

        private void autoComplete_SelectionChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (this.ItemSelected != null)
                MessageBox.Show(this.ItemSelected.Descricao);
            MessageBox.Show(this.SelectedValue);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.ItemSelected = new ItemComboInfo() { Codigo = "M3", Descricao = "MARLENE" };
            //this.SelectedValue = "M3";
            this.SelectedIndex = 0;
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info_)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info_));
            }
        }

        private void btnTesteEnabled_Click(object sender, RoutedEventArgs e)
        {
            //this.autoComplete.IsEnabled = !this.autoComplete.IsEnabled;
        }

        private void btnTrocarLista_Click(object sender, RoutedEventArgs e)
        {
            List<ItemComboInfo> lista = new List<ItemComboInfo>();

            //Dictionary<string, string> mock = new Dictionary<string, string>();
            lista.Add(new ItemComboInfo() { Codigo = "M1", Descricao = "Maria" }); //1
            lista.Add(new ItemComboInfo() { Codigo = "J1", Descricao = "João" });

            Lista2 = lista;
        }

        private void btnVoltarLista_Click(object sender, RoutedEventArgs e)
        {
            Lista2 = ListaInicial;
        }

        private void btnSelecionarItem_Click(object sender, RoutedEventArgs e)
        {
            this.ItemSelected3 = new ItemComboInfo() { Codigo = "M3", Descricao = "MARLENE" };
        }

        private void btnResetSelecao_Click(object sender, RoutedEventArgs e)
        {
            this.ItemSelected3 = null;
        }

        private void btnSelecionarItem4_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedValue4 = this.txtCodigoItem.Text;
        }

        private void btnSelecionarItem5_Click(object sender, RoutedEventArgs e)
        {
            int codigo = 0;
            if (int.TryParse(this.txtIndexItem.Text, out codigo))
                this.SelectedIndex5 = codigo;
        }

        private void btnZerarLista_Click(object sender, RoutedEventArgs e)
        {
            Lista6 = new List<ItemComboInfo>();
        }

        private void btnVoltarLista6_Click(object sender, RoutedEventArgs e)
        {
            Lista6 = ListaInicial;
        }

        private void btnMudarStatus7_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled7 = !this.IsEnabled7;
        }

        private void btnVisibility8_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility8 = Visibility.Visible;
        }

        private void btnCollapsed8_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility8 = Visibility.Collapsed;
        }

        private void btnHidden8_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility8 = Visibility.Hidden;
        }
    }
}
