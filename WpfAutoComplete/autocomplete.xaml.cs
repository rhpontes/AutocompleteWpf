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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace WpfAutoComplete
{
    /// <summary>
    /// Interaction logic for AutoComplete.xaml
    /// </summary>
    public partial class AutoComplete : UserControl, INotifyPropertyChanged
    {
        #region Propriedades

        public event PropertyChangedCallback SelectionChanged;

        private const string SELECIONE = "SELECIONE";
        private const string LABEL_LINK_DEFAULT = "SELECIONE";
        private const string LINK_COLOR_DEFAULT = "#0E5E79";

        private Dictionary<string, object> MapaObjects = new Dictionary<string, object>();

        private Dictionary<string, Item> MapaItems = new Dictionary<string, Item>();

        private List<Key> ListaNumerico = new List<Key>();

        /// <summary>
        /// Items informados ao componente
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable<object>),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null, OnItemsSourcePropertyChanged));

        // .NET Property wrapper
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            AutoComplete control = source as AutoComplete;
            System.Diagnostics.Debug.WriteLine("OnItemsSourcePropertyChanged: " + control.txt.Text);
            IEnumerable<object> lista = (IEnumerable<object>)e.NewValue;

            if (lista != null && control != null)
            {
                // Inicializa os mapas de referência para cada atribuição do source
                control.MapaObjects = new Dictionary<string, object>();
                control.MapaItems = new Dictionary<string, Item>();
                control.Items = new List<Item>();
                control.InternalItemSelected = null;
                control.ParseToItems(lista);
                control.MinimumSearchLength = 0;
                control.SetValue(HasItemsProperty,control.Items.Count() > 1);
            }
        }

        /// <summary>
        /// Items informados ao componente
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(
                "SelectedValuePath",
                typeof(string),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null));

        // .NET Property wrapper
        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        /// <summary>
        /// Items informados ao componente
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                "DisplayMemberPath",
                typeof(string),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null));

        // .NET Property wrapper
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Items informados ao componente
        /// </summary>
        private static readonly DependencyProperty InternalItemSelectedProperty =
            DependencyProperty.Register(
                "InternalItemSelected",
                typeof(Item),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null));


        // .NET Property wrapper
        private Item InternalItemSelected
        {
            get { return (Item)GetValue(InternalItemSelectedProperty); }
            set { SetValue(InternalItemSelectedProperty, value); }
        }

        /// <summary>
        /// Items informados ao componente
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null, OnSelectedItemPropertyChanged));

        // .NET Property wrapper
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); OnPropertyChanged("SelectedItem"); }
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            AutoComplete control = source as AutoComplete;
            System.Diagnostics.Debug.WriteLine("OnSelectedItemPropertyChanged: " + control.txt.Text);
            
            if (control.MapaObjects.Count == 0)
            {
                Item item = new Item() { Codigo = "null", Descricao = SELECIONE };
                control.Items.Add(item);
                control.MapaObjects.Add(item.Codigo, null);
                control.MapaItems.Add(item.Codigo, item);
            }
            else if (control.MapaObjects.ContainsKey(control.GetCodigoObject(e.NewValue)))
            {
                control.InternalItemSelected = control.MapaItems[control.GetCodigoObject(e.NewValue)];                    
                if (control.InternalItemSelected != null)
                {
                    control.SetValue(SelectedValueProperty, control.InternalItemSelected.Codigo);
                    control.SetValue(SelectedIndexProperty, control.InternalItemSelected.index);
                    control._bufferSearch.Clear();
                    if (!control._ehEntrada)
                    {
                        control.LabelLinkItemSelected = control.SetLabelLink(control.GetDescricaoObject(control.MapaObjects[control.InternalItemSelected.Codigo]));
                        control.txt.Text = control.InternalItemSelected.Descricao;
                        control._bufferSearch.Append(control.InternalItemSelected.Descricao);
                        control.FecharPopup();
                    }
                    else
                    {
                        control.txt.Text = "";
                    }
                    control.txtHolder.Text = "";
                    if (control.SelectionChanged != null)
                        control.SelectionChanged(control, e);
                }
            }
            
        }

        /// <summary>
        /// Items informados ao componente
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(
                "SelectedValue",
                typeof(object),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null, OnSelectedValuePropertyChanged));

        // .NET Property wrapper
        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); OnPropertyChanged("SelectedValue");}
        }

        private static void OnSelectedValuePropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            AutoComplete control = source as AutoComplete;
            System.Diagnostics.Debug.WriteLine("OnSelectedValuePropertyChanged: " + control.txt.Text);
            string codigo = "";
            if (e.NewValue == null || (e.NewValue != null && String.IsNullOrEmpty(e.NewValue.ToString())))
                codigo = "null";
            else
                codigo = e.NewValue.ToString();

            if (control.MapaObjects.ContainsKey(codigo))
            {
                
                if (control.InternalItemSelected == null ||
                    (control.InternalItemSelected != null &&
                        control.GetCodigoObject(control.MapaObjects[codigo]) != 
                        control.InternalItemSelected.Codigo)
                    )
                    control.SetValue(SelectedItemProperty, control.MapaObjects[codigo]);
            }
        }


        /// <summary>
        /// Items informados ao componente
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(Nullable<int>),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null, OnSelectedIndexPropertyChanged));

        // .NET Property wrapper
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); OnPropertyChanged("SelectedIndex"); }
        }

        private static void OnSelectedIndexPropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            AutoComplete control = source as AutoComplete;
            if (e.NewValue != null)
            {
                try
                {
                    int index = (int)e.NewValue;
                    if (index == -1)
                    {
                        control.SetValue(SelectedItemProperty, control.MapaObjects["null"]);
                    }
                    else if (control.InternalItemSelected == null || (control.InternalItemSelected != null 
                        && index != control.InternalItemSelected.index))
                    {
                        control.SetValue(SelectedItemProperty, control.GetObjectByIndex(index));
                    }
                    
                }
                catch (Exception ex)
                {

                }
            }
        }

        /// <summary>
        /// Items informados ao componente
        /// </summary>
        private static readonly DependencyProperty LabelLinkProperty =
            DependencyProperty.Register(
                "LabelLink",
                typeof(string),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null, OnLabelLinkPropertyChanged));

        public string LabelLink
        {
            get { return (string)GetValue(LabelLinkProperty); }
            set { SetValue(LabelLinkProperty, value); }
        }

        private static void OnLabelLinkPropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            AutoComplete control = source as AutoComplete;
            if (e.NewValue != null)
            {
                if (String.IsNullOrEmpty(control.LabelLinkItemSelected)
                    || (control.LabelLinkItemSelected != null && control.LabelLinkItemSelected == LABEL_LINK_DEFAULT))
                    control.LabelLinkItemSelected = e.NewValue.ToString();
            }
        }

        /// <summary>
        /// Items informados ao componente
        /// </summary>
        private static readonly DependencyProperty LabelLinkItemSelectedProperty =
            DependencyProperty.Register(
                "LabelLinkItemSelected",
                typeof(string),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null));

        private string LabelLinkItemSelected
        {
            get { return (string)GetValue(LabelLinkItemSelectedProperty); }
            set { SetValue(LabelLinkItemSelectedProperty, value); }
        }

        /// <summary>
        /// Items informados ao componente
        /// </summary>
        public static readonly DependencyProperty ColorLinkProperty =
            DependencyProperty.Register(
                "ColorLink",
                typeof(Brush),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null));

        public Brush ColorLink
        {
            get { return (Brush)GetValue(ColorLinkProperty); }
            set { SetValue(ColorLinkProperty, value); }
        }

        /// <summary>
        /// Items informados ao componente
        /// </summary>
        public static readonly DependencyProperty HasItemsProperty =
            DependencyProperty.Register(
                "HasItems",
                typeof(bool),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(false));

        public bool HasItems
        {
            get { return (bool)GetValue(HasItemsProperty); }
            set { SetValue(HasItemsProperty, (this.Items.Count > 1)); }
        }


        /// <summary>
        /// Quantidade minima de caracteres para busca
        /// </summary>
        private static readonly DependencyProperty MinimumSearchLengthProperty =
            DependencyProperty.Register(
                "MinimumSearchLength",
                typeof(int),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null));

        private int MinimumSearchLength
        {
            get { return (int)GetValue(MinimumSearchLengthProperty); }
            set { SetValue(MinimumSearchLengthProperty, value); }
        }

        /// <summary>
        /// Posição do popup
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register(
                "Placement",
                typeof(string),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata("Bottom", OnPlacementPropertyChanged));

        public string Placement
        {
            get { return (string)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        private static void OnPlacementPropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            AutoComplete control = source as AutoComplete;
            if (e.NewValue != null)
            {
                switch (e.NewValue.ToString().ToUpper())
                {
                    case "TOP" :
                        control.InternalPlacement = PlacementMode.Top;
                        break;
                    case "LEFT" :
                        control.InternalPlacement = PlacementMode.Left;
                        break;
                    default:
                        control.InternalPlacement = PlacementMode.Bottom;
                        break;
                }
            }
        }

        /// <summary>
        /// Posição do popup
        /// </summary>
        private static readonly DependencyProperty InternalPlacementProperty =
            DependencyProperty.Register(
                "InternalPlacement",
                typeof(PlacementMode),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(PlacementMode.Bottom));

        private PlacementMode InternalPlacement
        {
            get { return (PlacementMode)GetValue(InternalPlacementProperty); }
            set { SetValue(InternalPlacementProperty, value); }
        }

        /// <summary>
        /// Items que serão mostrados no ListBox
        /// </summary>
        private List<Item> Items = new List<Item>();

        /// <summary>
        /// Lista de items filtrados e tratados
        /// </summary>
        private List<Item> ListaFiltrada = new List<Item>();

        private Visibility _linkEnabled = Visibility.Visible;
        public Visibility LinkEnabled
        {
            get { return _linkEnabled; }
            set { _linkEnabled = value; OnPropertyChanged("LinkEnabled"); }
        }

        private Visibility _linkDisabled = Visibility.Collapsed;
        public Visibility LinkDisabled
        {
            get { return _linkDisabled; }
            set { _linkDisabled = value; OnPropertyChanged("LinkDisabled"); }
        }


        /// <summary>
        /// Posição do popup
        /// </summary>
        private static readonly DependencyProperty MaxWidthBoxProperty =
            DependencyProperty.Register(
                "MaxWidthBox",
                typeof(Double?),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null));

        public Double? MaxWidthBox
        {
            get { return (Double?)GetValue(MaxWidthBoxProperty); }
            set { SetValue(MaxWidthBoxProperty, value); }
        }

        private double _internalMaxWidth = 0;
        private double InternalMaxWidth 
        {
            get { return _internalMaxWidth; }
            set { _internalMaxWidth = value; OnPropertyChanged("InternalMaxWidth"); }
        }


        private StringBuilder _bufferSearch = new StringBuilder();
        private bool _ehEntrada = false;
        private bool _ehSaida = true;

        #endregion   

        #region Construtor

        /// <summary>
        /// Construtor Default
        /// </summary>
        public AutoComplete()
        {
            InitializeComponent();
            this.DataContext = this;

            ListaNumerico.Add(Key.D0);
            ListaNumerico.Add(Key.D1);
            ListaNumerico.Add(Key.D2);
            ListaNumerico.Add(Key.D3);
            ListaNumerico.Add(Key.D4);
            ListaNumerico.Add(Key.D5);
            ListaNumerico.Add(Key.D6);
            ListaNumerico.Add(Key.D7);
            ListaNumerico.Add(Key.D8);
            ListaNumerico.Add(Key.D9);
            ListaNumerico.Add(Key.NumPad0);
            ListaNumerico.Add(Key.NumPad1);
            ListaNumerico.Add(Key.NumPad2);
            ListaNumerico.Add(Key.NumPad3);
            ListaNumerico.Add(Key.NumPad4);
            ListaNumerico.Add(Key.NumPad5);
            ListaNumerico.Add(Key.NumPad6);
            ListaNumerico.Add(Key.NumPad7);
            ListaNumerico.Add(Key.NumPad8);
            ListaNumerico.Add(Key.NumPad9);

            // Label link default
            LabelLink = LABEL_LINK_DEFAULT;

            //InternalLabelLink = LABEL_LINK_DEFAULT;
            var converter = new System.Windows.Media.BrushConverter();
            // Cor font link default
            ColorLink = (Brush)converter.ConvertFromString(LINK_COLOR_DEFAULT);

            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler((sender, e) => {
                System.Diagnostics.Debug.WriteLine("IsEnabledChanged: " + txt.Text);
                if (Convert.ToBoolean(e.NewValue))
                {
                    btnLink.Style = (Style)this.Resources["Link"];
                }
                else
                {
                    if (InternalItemSelected != null)
                    {
                        _bufferSearch.Clear();
                        _bufferSearch.Append(InternalItemSelected.Descricao);
                        LabelLinkItemSelected = SetLabelLink(GetDescricaoObject(MapaObjects[InternalItemSelected.Codigo]));
                        txt.CaretIndex = txt.Text.Length;
                        SetValue(SelectedItemProperty, MapaObjects[InternalItemSelected.Codigo]);
                        FecharPopup();
                    }
                    btnLink.Style = (Style)this.Resources["LinkDisabled"];
                }

            });

            this.PreviewKeyDown += new KeyEventHandler((sender, e) => {
                System.Diagnostics.Debug.WriteLine("PreviewKeyDown: " + txt.Text);
                if (e.Key == Key.LeftAlt || e.Key == Key.System)
                {
                    this.FecharPopup();
                    e.Handled = true;
                }
            });
        }

        #endregion

        #region Metodos Privados

        private void AjustarSarchBuffer(string letra)
        {
            if (!String.IsNullOrEmpty(letra))
            {
                Regex regexSearch = new Regex(@"^[\r|\t]$");
                if (!regexSearch.IsMatch(letra.ToUpper()))
                    //Add letra no buffer de pesquisa
                    _bufferSearch.Append(letra.ToUpper());
            }
        }

        /// <summary>
        /// Filtra de acordo com o texto informado
        /// </summary>
        /// <param name="letra"></param>

        private void AjustarSarchBuffer(Key key)
        {
            string strKey = key.ToString();

            if (key == Key.Back)
            {
                if (_bufferSearch.Length == 1)
                {
                    _bufferSearch = _bufferSearch.Clear();
                }
                else if (_bufferSearch.Length > 1)
                {
                    _bufferSearch = _bufferSearch.Remove((_bufferSearch.Length - 1), 1);
                }
                
            }

            else if (key == Key.Space)
            {
                _bufferSearch.Append(" ");
            }

            else if (ListaNumerico.Contains(key))
            {
                if (strKey.Length > 5)
                    strKey = strKey.Substring(5);
                else
                    strKey = strKey.Substring(1);
            }
        }

        private void Filtrar(string letra)
        {
            System.Diagnostics.Debug.WriteLine("Filtrar: " + txt.Text);
            ListaFiltrada = new List<Item>();

            // Monta o regex de busca
            Regex regex = new Regex(@"^" + Regex.Escape(_bufferSearch.ToString()));

            if (_bufferSearch.Length >= MinimumSearchLength)
            {
                foreach (Item item in this.Items)
                {
                    if (regex.IsMatch(item.Descricao.ToUpper()))
                        ListaFiltrada.Add(new Item() { Codigo = item.Codigo, Descricao = item.Descricao.ToUpper() });
                }
            }

            if (String.IsNullOrEmpty(_bufferSearch.ToString()))
                ListaFiltrada = new List<Item>(this.Items);
            lista.ItemsSource = ListaFiltrada;
            lista.Items.Refresh();
            TratarTexto(letra);

            if (ListaFiltrada.Count > 0)
                OpenPopup();
            else
                FecharPopup(false);

        }

        /// <summary>
        /// Trata o efeito de complemento do texto
        /// </summary>
        /// <param name="listaFiltrada"></param>
        /// <param name="letra"></param>
        private void TratarTexto(string letra)
        {
            string textoItem = ListaFiltrada.Count > 0 ? ListaFiltrada[0].Descricao : "";

            if (!String.IsNullOrEmpty(textoItem))
            {
                txtHolder.Text = textoItem;
            }
            else
            {
                txtHolder.Text = "";
            }

        }

        /// <summary>
        /// Faz o parse do dicionários em itens
        /// </summary>
        /// <param name="itemsSourceFiltrado"></param>
        private void ParseToItems(IEnumerable<object> itemsSource)
        {
            Item itemSelecione = new Item() { Codigo = "null", Descricao = SELECIONE, index = -1 };
            Items.Add(itemSelecione);
            MapaObjects.Add(itemSelecione.Codigo, null);
            MapaItems.Add(itemSelecione.Codigo, itemSelecione);
            int contIndex = 0;
            foreach (object obj in itemsSource)
            {
                Item item = ParseToItem(obj);
                item.index = contIndex;
                Items.Add(item);
                contIndex++;
            }
        }

        private Item ParseToItem(object obj)
        {
            Type objType = obj.GetType();
            string codigo = "";
            if (obj.GetType().InvokeMember(SelectedValuePath, BindingFlags.GetProperty, null, obj, null) != null)
                codigo = obj.GetType().InvokeMember(SelectedValuePath, BindingFlags.GetProperty, null, obj, null).ToString();
            string descricao = "";
            if (obj.GetType().InvokeMember(DisplayMemberPath, BindingFlags.GetProperty, null, obj, null) != null)
                descricao = obj.GetType().InvokeMember(DisplayMemberPath, BindingFlags.GetProperty, null, obj, null).ToString().ToUpper();

            Item item = new Item() { Codigo = codigo, Descricao = descricao };

            // Add obj no mapa de referencia
            if (!MapaObjects.ContainsKey(item.Codigo))
                MapaObjects.Add(item.Codigo, obj);

            // Add item no mapa de referência
            if (!MapaItems.ContainsKey(item.Codigo))
                MapaItems.Add(item.Codigo, item);

            return item;
        }

        /// <summary>
        /// Faz o parse do dicionários em itens
        /// </summary>
        /// <param name="itemsSourceFiltrado"></param>
        private void ParseToObject(Dictionary<string, string> itemsSourceFiltrado)
        {
            List<Item> lista = new List<Item>();

            foreach (KeyValuePair<string, string> entry in itemsSourceFiltrado)
            {
                lista.Add(new Item() { Codigo = entry.Key, Descricao = entry.Value.ToUpper() });
            }

            Items = lista;
        }

        private string GetCodigoObject(object obj)
        {
            if (obj == null || String.IsNullOrEmpty(SelectedValuePath))
                return "null";

            string codigo = "";
            if (obj.GetType().InvokeMember(SelectedValuePath, BindingFlags.GetProperty, null, obj, null) != null)
                codigo = obj.GetType().InvokeMember(SelectedValuePath, BindingFlags.GetProperty, null, obj, null).ToString();
            return codigo;
        }

        private string GetDescricaoObject(object obj)
        {
            if (obj == null)
                return SELECIONE;

            if (String.IsNullOrEmpty(DisplayMemberPath))
                return "";

            string descricao = "";
            if (obj.GetType().InvokeMember(DisplayMemberPath, BindingFlags.GetProperty, null, obj, null) != null)
                descricao = obj.GetType().InvokeMember(DisplayMemberPath, BindingFlags.GetProperty, null, obj, null).ToString();
            return descricao;
        }

        private object GetObjectByIndex(int index)
        {
            if (Items.Count > 0)
            {
                Item item = Items.Where(x => x.index == index).First();
                if (item != null)
                    return MapaObjects[item.Codigo];
            }
            return null;
        }

        #endregion

        #region Metodos Publicos

        public void TextClear()
        {
            txt.Text = "";
        }

        public void FecharPopup(bool showLink)
        {
           if (popup.IsOpen == true)
                popup.IsOpen = false;

            if (showLink)
            {
                LinkEnabled = Visibility.Visible;
                LinkDisabled = Visibility.Collapsed;
                this.btnLink.Focus();
            }

            if (InternalItemSelected != null && _ehSaida)
                LabelLinkItemSelected = SetLabelLink(GetDescricaoObject(MapaObjects[InternalItemSelected.Codigo]));
            else
                LabelLinkItemSelected = LabelLink;
        }

        public void FecharPopup()
        {
            FecharPopup(true);
        }

        private void OpenPopup()
        {
            System.Diagnostics.Debug.WriteLine("OpenPopup: " + txt.Text);
            if (ListaFiltrada.Count > 0 && _bufferSearch.Length >= MinimumSearchLength)
            {
                if (this.InternalPlacement == PlacementMode.Left)
                {
                    this.popup.VerticalOffset = this.ActualHeight;
                    this.popup.HorizontalOffset = this.ActualWidth;
                }
                else if (this.InternalPlacement == PlacementMode.Bottom)
                {
                    this.popup.VerticalOffset = 0;
                    this.popup.HorizontalOffset = 0;
                }

                if (this.MaxWidthBox != null)
                {
                    if (this.MaxWidthBox < this.txt.ActualWidth)
                        this.popup.MaxWidth = this.txt.ActualWidth;
                    else
                        this.popup.MaxWidth = (double)this.MaxWidthBox;
                }

                popup.IsOpen = true;
                LinkEnabled = Visibility.Collapsed;
                LinkDisabled = Visibility.Visible;
                this.scroolViewer.ScrollToTop();
                this.scroolViewer.ScrollToLeftEnd();
            }

        }

        private string SetLabelLink(string descricao)
        {
            if (!String.IsNullOrEmpty(descricao) && descricao != SELECIONE)
                return descricao;
            else
                return LabelLink;
        }

        #endregion

        #region Eventos

        /// <summary>
        /// Trata o evento de entrada de texto no input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("txt_PreviewTextInput: " + txt.Text);
            // Filtra a lista de items
            AjustarSarchBuffer(e.Text);
            Filtrar(e.Text);
        }

        private void txt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("txt_PreviewKeyDown: " + txt.Text);

            if (_ehEntrada)
            {
                _bufferSearch.Append(txt.Text);
                _ehEntrada = false;
            }

            if (e.Key == Key.Escape || e.Key == Key.Delete)
            {
                e.Handled = true;
                return;
            }

            if (e.Key != Key.Enter && e.Key != Key.Tab)
            {
                AjustarSarchBuffer(e.Key);
                Filtrar("");
            }

           if (e.Key == Key.Down)
            {
                if (Items != null && Items.Count > 0)
                {
                    OpenPopup();
                    lista.Focus();
                    lista.SelectedItem = ListaFiltrada.FirstOrDefault();
                    lista.UpdateLayout();

                    ListBoxItem listItem = null;
                    listItem = (ListBoxItem)lista.ItemContainerGenerator.ContainerFromItem(lista.SelectedItem);
                    if (listItem != null)
                    {
                            
                        listItem.Focus();
                        TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Previous);
                        UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;
                        if (keyboardFocus != null)
                        {
                            keyboardFocus.MoveFocus(tRequest);
                        }
                    }

                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Tab)
            {
                _ehEntrada = false;
                _ehSaida = true;
                InternalItemSelected = ListaFiltrada.FirstOrDefault() != null ? ListaFiltrada.FirstOrDefault() : null;
                if (InternalItemSelected != null)
                {
                    _bufferSearch.Clear();
                    _bufferSearch.Append(InternalItemSelected.Descricao);
                    LabelLinkItemSelected = SetLabelLink(GetDescricaoObject(MapaObjects[InternalItemSelected.Codigo]));
                    txt.CaretIndex = txt.Text.Length;
                    SetValue(SelectedItemProperty, MapaObjects[InternalItemSelected.Codigo]);
                    FecharPopup();
                }
                else
                {
                    _bufferSearch.Clear();
                    SetValue(SelectedItemProperty, MapaObjects["null"]);
                    LabelLinkItemSelected = SetLabelLink(GetDescricaoObject(MapaObjects["null"]));
                    FecharPopup();
                }
                

            }
            else if (e.Key == Key.Enter)
            {
                _ehEntrada = false;
                _ehSaida = true;
                InternalItemSelected = ListaFiltrada.FirstOrDefault() != null ? ListaFiltrada.FirstOrDefault() : null;
                if (InternalItemSelected != null)
                {
                    _bufferSearch.Clear();
                    _bufferSearch.Append(InternalItemSelected.Descricao);
                    LabelLinkItemSelected = SetLabelLink(GetDescricaoObject(MapaObjects[InternalItemSelected.Codigo]));
                    txt.CaretIndex = txt.Text.Length;
                    SetValue(SelectedItemProperty, MapaObjects[InternalItemSelected.Codigo]);
                    FecharPopup();
                    e.Handled = true;
                        
                }
                else
                {
                    _bufferSearch.Clear();
                    SetValue(SelectedItemProperty, MapaObjects["null"]);
                    FecharPopup();
                    e.Handled = true;
                }
                
            }
           else if (e.Key == Key.Right)
           {
                InternalItemSelected = ListaFiltrada.FirstOrDefault() != null ? ListaFiltrada.FirstOrDefault() : null;
                if (InternalItemSelected != null)
                {
                    _bufferSearch.Clear();
                    _bufferSearch.Append(InternalItemSelected.Descricao);
                    LabelLinkItemSelected = SetLabelLink(GetDescricaoObject(MapaObjects[InternalItemSelected.Codigo]));
                    txt.CaretIndex = txt.Text.Length;
                    SetValue(SelectedItemProperty, MapaObjects[InternalItemSelected.Codigo]);
                    _ehEntrada = false;
                    _ehSaida = true;
                    FecharPopup();
                }
           }
           else if (e.Key == Key.Back && txt.Text.Length == 1)
           {
               txt.Text = "";
               txtHolder.Text = "";
               _bufferSearch.Clear();
               txt.Focus();
              
           }
           else if (e.Key == Key.Back && txt.SelectionLength == txt.Text.Length)
           {
               txt.Text = "";
               txtHolder.Text = "";
               _bufferSearch.Clear();
               Filtrar("");             
           }
           else if (e.Key == Key.Back && txt.SelectionLength > 0)
           {
               Regex regex = new Regex(Regex.Escape(txt.SelectedText));
               string resultado = regex.Replace(txt.Text, "");
               _bufferSearch.Clear();
               if (resultado.Length > 0)
                   _bufferSearch.Append(resultado);
               
               txt.Text = resultado;
               txtHolder.Text = "";
               txt.CaretIndex = txt.Text.Length;
               Filtrar("");
               e.Handled = true;
           }
           else if (e.Key == Key.Back && this._bufferSearch.Length < MinimumSearchLength)
           {
               FecharPopup(false);
           }

        }

        /// <summary>
        /// Trata o evento de mudança de seleção
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("lista_SelectionChanged: " + txt.Text);
            ListBox listBox = sender as ListBox;
            
            if (e.AddedItems.Count > 0 && InternalItemSelected != null)
            {
                txt.Text = ((Item)e.AddedItems[0]).Descricao;
                txtHolder.Text = "";
                _bufferSearch.Clear();
                LabelLinkItemSelected = SetLabelLink(GetDescricaoObject(MapaObjects[((Item)e.AddedItems[0]).Codigo]));
            }
        }

        private void lista_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("lista_PreviewKeyDown: " + txt.Text);
            if (e.Key == Key.Tab || e.Key == Key.Enter)
            {
                _ehEntrada = false;
                _ehSaida = true;
                FecharPopup();
                txtHolder.Text = "";
                
                ListBox lisbox = sender as ListBox;
                if (lisbox != null && lisbox.SelectedItem != null)
                {
                    
                    _bufferSearch.Clear();
                    _bufferSearch.Append(((Item)lisbox.SelectedItem).Descricao);
                    LabelLinkItemSelected = SetLabelLink(GetDescricaoObject(MapaObjects[((Item)lisbox.SelectedItem).Codigo]));
                    txt.Text = "";
                    txt.CaretIndex = txt.Text.Length;
                    SetValue(SelectedItemProperty, MapaObjects[((Item)lisbox.SelectedItem).Codigo]);
                    ListaFiltrada = new List<Item>(this.Items);
                    lista.ItemsSource = ListaFiltrada;
                    lista.Items.Refresh();                    
                }

                e.Handled = true;
            }
        }

        #endregion

        #region Member

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));

        }

        #endregion

        private void txt_LostFocus(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("txt_LostFocus: " + txt.Text);
            InternalItemSelected = ListaFiltrada.FirstOrDefault() != null ? ListaFiltrada.FirstOrDefault() : null;
            txt.CaretIndex = txt.Text.Length;
            if (!(grd.IsKeyboardFocusWithin || popup.IsKeyboardFocusWithin))
                FecharPopup();
        }        

        private void Button_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button_GotFocus: " + txt.Text);
        }

        /// <summary>
        /// Trata o evento do click no link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Link_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Link_Click: " + txt.Text);
            LinkEnabled = Visibility.Collapsed;
            LinkDisabled = Visibility.Visible;
            if (InternalItemSelected == null || (InternalItemSelected != null && InternalItemSelected.Codigo == "null"))
            {
                txt.Text = "";
                txtHolder.Text = "";
            }
            else
            {
                txt.Text = InternalItemSelected.Descricao;
            }
            _ehEntrada = true;
            txt.Focus();
        }

        private void txt_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("txt_GotFocus: " + txt.Text);

            _ehSaida = false;
            _bufferSearch.Clear();

            if (!_ehEntrada)
            {
                _bufferSearch.Append(txt.Text);
                if (InternalItemSelected != null && !_ehEntrada)
                    txtHolder.Text = InternalItemSelected.Descricao;
                txt.CaretIndex = txt.Text.Length;
                Filtrar("");
            }
            else
            {
                txt.Text = "";
                this.InternalItemSelected = null;
                SetValue(SelectedItemProperty, MapaObjects["null"]);
                txt.CaretIndex = txt.Text.Length;
                ListaFiltrada = new List<Item>(this.Items);
                lista.ItemsSource = ListaFiltrada;
                lista.Items.Refresh();
                Filtrar("");
            }

            
            //_ehEntrada = false;
        }

        private void lista_LostFocus(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("lista_LostFocus: " + txt.Text);
            if (!(grd.IsKeyboardFocusWithin || popup.IsKeyboardFocusWithin))
                FecharPopup();
        }

        private void txt_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("txt_Pasting: " + txt.Text);
            e.CancelCommand();
        }

        private void btnLink_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnLink_PreviewKeyDown: " + txt.Text);
            if (e.Key == Key.Enter)
            {
                if (InternalItemSelected == null || (InternalItemSelected != null && InternalItemSelected.Codigo == "null"))
                {
                    txt.Text = "";
                    txtHolder.Text = "";
                }
                else
                {
                    txt.Text = InternalItemSelected.Descricao;
                }

                _bufferSearch.Clear();
                Filtrar("");
                _ehEntrada = true;
            }
        }

        private void btnLink_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnLink_PreviewMouseDown: " + txt.Text);
            if (InternalItemSelected == null || (InternalItemSelected != null && InternalItemSelected.Codigo == "null"))
            {
                txt.Text = "";
                txtHolder.Text = "";
            }
            else
            {
                txt.Text = InternalItemSelected.Descricao;
            }
            _bufferSearch.Clear();
            Filtrar("");
            _ehEntrada = true;
            txt.Focus();
        }

        private void lista_LostMouseCapture(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("lista_LostMouseCapture: ");
            ListBox lisbox = sender as ListBox;
            if (lisbox != null && lisbox.SelectedItem != null)
            {
                Item itemSelected = (Item)lisbox.SelectedItem;
                _bufferSearch.Clear();
                _bufferSearch.Append(itemSelected.Descricao);
                LabelLinkItemSelected = SetLabelLink(GetDescricaoObject(MapaObjects[itemSelected.Codigo]));
                SetValue(SelectedItemProperty, MapaObjects[itemSelected.Codigo]);
                txt.Text = "";
                txt.CaretIndex = txt.Text.Length;
                ListaFiltrada = new List<Item>(this.Items);
                lista.ItemsSource = ListaFiltrada;
                lista.Items.Refresh();
                InternalItemSelected = itemSelected;
                _ehSaida = true;
                _ehEntrada = false;
                FecharPopup();
            }
        }
        
    }


    /// <summary>
    /// Classe para representar os items do combo
    /// </summary>
    public class Item
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public int index { get; set; }
    }
}
