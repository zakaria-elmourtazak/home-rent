// Product Data
const products = [
    {
        id: 1,
        name: 'Premium Headphones',
        price: 199.99,
        image: 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=300&h=300&fit=crop',
        rating: 5,
        description: 'High-quality audio with noise cancellation'
    },
    {
        id: 2,
        name: 'Wireless Mouse',
        price: 49.99,
        image: 'https://images.unsplash.com/photo-1527814050087-3793815479db?w=300&h=300&fit=crop',
        rating: 4,
        description: 'Ergonomic design with long battery life'
    },
    {
        id: 3,
        name: 'USB-C Hub',
        price: 79.99,
        image: 'https://images.unsplash.com/photo-1625948515291-69613efd103f?w=300&h=300&fit=crop',
        rating: 5,
        description: 'Multi-port connectivity solution'
    },
    {
        id: 4,
        name: 'Mechanical Keyboard',
        price: 149.99,
        image: 'https://images.unsplash.com/photo-1587829191301-26ec98241487?w=300&h=300&fit=crop',
        rating: 5,
        description: 'Premium switches with RGB lighting'
    },
    {
        id: 5,
        name: 'Portable Charger',
        price: 59.99,
        image: 'https://images.unsplash.com/photo-1609091839311-d5365f9ff1c5?w=300&h=300&fit=crop',
        rating: 4,
        description: '20000mAh fast charging battery'
    },
    {
        id: 6,
        name: 'Webcam 4K',
        price: 129.99,
        image: 'https://images.unsplash.com/photo-1598921915826-5b83c426c5a6?w=300&h=300&fit=crop',
        rating: 5,
        description: 'Crystal clear 4K video quality'
    }
];

// Load Products
function loadProducts() {
    const productsGrid = document.getElementById('productsGrid');
    if (!productsGrid) return;

    productsGrid.innerHTML = products.map(product => `
        <div class="col-md-6 col-lg-4">
            <div class="product-card">
                <img src="${product.image}" alt="${product.name}" class="product-image">
                <div class="product-info">
                    <h3 class="product-title">${product.name}</h3>
                    <div class="product-rating">
                        ${'★'.repeat(product.rating)}${'☆'.repeat(5 - product.rating)}
                    </div>
                    <div class="product-price">$${product.price.toFixed(2)}</div>
                    <p class="product-description">${product.description}</p>
                    <div class="product-actions">
                        <button class="btn btn-primary btn-sm" onclick="addToCart(${product.id})">Add to Cart</button>
                        <button class="btn btn-outline-primary btn-sm">View Details</button>
                    </div>
                </div>
            </div>
        </div>
    `).join('');
}

// Add to Cart
function addToCart(productId) {
    const product = products.find(p => p.id === productId);
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    
    const existingItem = cart.find(item => item.id === productId);
    if (existingItem) {
        existingItem.quantity++;
    } else {
        cart.push({ ...product, quantity: 1 });
    }
    
    localStorage.setItem('cart', JSON.stringify(cart));
    showNotification('Product added to cart!');
}

// Load Cart
function loadCart() {
    const cartItemsDiv = document.getElementById('cartItems');
    if (!cartItemsDiv) return;

    let cart = JSON.parse(localStorage.getItem('cart')) || [];

    if (cart.length === 0) {
        cartItemsDiv.innerHTML = `
            <div class="empty-cart">
                <div class="empty-cart-icon">🛒</div>
                <h3>Your cart is empty</h3>
                <p class="text-muted">Add some products to get started</p>
                <a href="products.html" class="btn btn-primary mt-3">Continue Shopping</a>
            </div>
        `;
        return;
    }

    cartItemsDiv.innerHTML = cart.map(item => `
        <div class="cart-item">
            <img src="${item.image}" alt="${item.name}" class="cart-item-image">
            <div class="cart-item-details">
                <div class="cart-item-title">${item.name}</div>
                <div class="cart-item-price">$${item.price.toFixed(2)}</div>
                <div class="quantity-control">
                    <button onclick="updateQuantity(${item.id}, -1)">−</button>
                    <span>${item.quantity}</span>
                    <button onclick="updateQuantity(${item.id}, 1)">+</button>
                    <span class="ms-auto remove-btn" onclick="removeFromCart(${item.id})">Remove</span>
                </div>
            </div>
            <div class="text-end">
                <strong>$${(item.price * item.quantity).toFixed(2)}</strong>
            </div>
        </div>
    `).join('');

    updateCartSummary();
}

// Update Cart Summary
function updateCartSummary() {
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    const subtotal = cart.reduce((sum, item) => sum + item.price * item.quantity, 0);
    const shipping = subtotal > 100 ? 0 : 15;
    const tax = subtotal * 0.08;
    const total = subtotal + shipping + tax;

    document.getElementById('subtotal').textContent = `$${subtotal.toFixed(2)}`;
    document.getElementById('shipping').textContent = `$${shipping.toFixed(2)}`;
    document.getElementById('tax').textContent = `$${tax.toFixed(2)}`;
    document.getElementById('total').textContent = `$${total.toFixed(2)}`;
}

// Update Quantity
function updateQuantity(productId, change) {
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    const item = cart.find(p => p.id === productId);
    if (item) {
        item.quantity += change;
        if (item.quantity <= 0) {
            cart = cart.filter(p => p.id !== productId);
        }
    }
    localStorage.setItem('cart', JSON.stringify(cart));
    loadCart();
}

// Remove from Cart
function removeFromCart(productId) {
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    cart = cart.filter(p => p.id !== productId);
    localStorage.setItem('cart', JSON.stringify(cart));
    loadCart();
}

// Show Notification
function showNotification(message) {
    const notification = document.createElement('div');
    notification.textContent = message;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: linear-gradient(135deg, #6366f1 0%, #ec4899 100%);
        color: white;
        padding: 1rem 2rem;
        border-radius: 10px;
        box-shadow: 0 8px 20px rgba(0, 0, 0, 0.2);
        z-index: 9999;
        animation: slideInRight 0.3s ease-out;
    `;
    document.body.appendChild(notification);
    setTimeout(() => notification.remove(), 3000);
}

// Initialize on Page Load
document.addEventListener('DOMContentLoaded', () => {
    loadProducts();
    loadCart();
});

// Add CSS for notification animation
const style = document.createElement('style');
style.textContent = `
    @keyframes slideInRight {
        from {
            opacity: 0;
            transform: translateX(100px);
        }
        to {
            opacity: 1;
            transform: translateX(0);
        }
    }
`;
document.head.appendChild(style);
//   tailwind.config = {
//             theme: {
//                 extend: {
//                     colors: {
//                         primary: '#3b82f6',
//                         secondary: '#1e40af',
//                         accent: '#f97316'
//                     }
//                 }
//             }
//         }
     document.getElementById('image-upload').addEventListener('change', function(e) {
            const files = e.target.files;
            const previewContainer = document.getElementById('image-preview');
            
            // Clear previous previews
            previewContainer.innerHTML = '';
            
            // Display previews for each selected image
            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                if (!file.type.match('image.*')) continue;
                
                const reader = new FileReader();
                reader.onload = function(e) {
                    const previewItem = document.createElement('div');
                    previewItem.className = 'image-preview-item';
                    
                    const img = document.createElement('img');
                    img.src = e.target.result;
                    img.alt = 'Property Image';
                    
                    const removeBtn = document.createElement('span');
                    removeBtn.className = 'remove-image';
                    removeBtn.innerHTML = '&times;';
                    removeBtn.onclick = function() {
                        previewItem.remove();
                    };
                    
                    previewItem.appendChild(img);
                    previewItem.appendChild(removeBtn);
                    previewContainer.appendChild(previewItem);
                };
                reader.readAsDataURL(file);
            }
        });
        
        // Form submission
        document.getElementById('property-form').addEventListener('submit', function(e) {
            e.preventDefault();
            
            // In a real application, you would send the form data to a server here
            alert('Property submitted successfully! In a real application, this would be sent to our servers.');
            
            // Reset form
            this.reset();
            document.getElementById('image-preview').innerHTML = '';


        });
         const addPropertyBtn = document.getElementById('add-property-btn');
        const propertyFormContainer = document.getElementById('property-form-container');
        const formTitle = document.getElementById('form-title');
        const propertyForm = document.getElementById('property-form');
        const cancelBtn = document.getElementById('cancel-btn');
        const imageUpload = document.getElementById('image-upload');
        const imagePreview = document.getElementById('image-preview');
        
        // Show add property form
        addPropertyBtn.addEventListener('click', () => {
            formTitle.textContent = 'Add New Property';
            propertyFormContainer.classList.remove('hidden');
            propertyForm.reset();
            imagePreview.innerHTML = '';
            document.getElementById('property-id').value = '';
        });
        
        // Hide form
        cancelBtn.addEventListener('click', () => {
            propertyFormContainer.classList.add('hidden');
        });
        
        // Image upload and preview functionality
        imageUpload.addEventListener('change', function(e) {
            const files = e.target.files;
            
            // Clear previous previews
            imagePreview.innerHTML = '';
            
            // Display previews for each selected image
            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                if (!file.type.match('image.*')) continue;
                
                const reader = new FileReader();
                reader.onload = function(e) {
                    const previewItem = document.createElement('div');
                    previewItem.className = 'image-preview-item';
                    
                    const img = document.createElement('img');
                    img.src = e.target.result;
                    img.alt = 'Property Image';
                    
                    const removeBtn = document.createElement('span');
                    removeBtn.className = 'remove-image';
                    removeBtn.innerHTML = '&times;';
                    removeBtn.onclick = function() {
                        previewItem.remove();
                    };
                    
                    previewItem.appendChild(img);
                    previewItem.appendChild(removeBtn);
                    imagePreview.appendChild(previewItem);
                };
                reader.readAsDataURL(file);
            }
        });
        
        // Form submission
        propertyForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            // Get form data
            const propertyId = document.getElementById('property-id').value;
            const title = document.getElementById('property-title').value;
            const description = document.getElementById('property-description').value;
            const type = document.getElementById('property-type').value;
            const price = document.getElementById('property-price').value;
            const bedrooms = document.getElementById('property-bedrooms').value;
            const bathrooms = document.getElementById('property-bathrooms').value;
            const sqft = document.getElementById('property-sqft').value;
            const date = document.getElementById('property-date').value;
            const street = document.getElementById('property-street').value;
            const city = document.getElementById('property-city').value;
            const state = document.getElementById('property-state').value;
            const zip = document.getElementById('property-zip').value;
            
            // In a real application, you would send this data to a server
            if (propertyId) {
                // Update existing property
                alert(`Property "${title}" updated successfully!`);
            } else {
                // Add new property
                alert(`Property "${title}" added successfully!`);
            }
            
            // Hide form
            propertyFormContainer.classList.add('hidden');
        });
        
        // Edit property
        document.querySelectorAll('.edit-btn').forEach(button => {
            button.addEventListener('click', function() {
                formTitle.textContent = 'Edit Property';
                propertyFormContainer.classList.remove('hidden');
                
                // In a real application, you would populate the form with existing data
                document.getElementById('property-id').value = '1';
                document.getElementById('property-title').value = 'Modern Apartment Downtown';
                document.getElementById('property-description').value = 'Beautiful modern apartment in the heart of downtown with amazing city views.';
                document.getElementById('property-type').value = 'Apartment';
                document.getElementById('property-price').value = '1200';
                document.getElementById('property-bedrooms').value = '2';
                document.getElementById('property-bathrooms').value = '1';
                document.getElementById('property-sqft').value = '900';
                document.getElementById('property-date').value = '2023-06-15';
                document.getElementById('property-street').value = '123 Main Street';
                document.getElementById('property-city').value = 'New York';
                document.getElementById('property-state').value = 'NY';
                document.getElementById('property-zip').value = '10001';
            });
        });
        
        // Delete property
        document.querySelectorAll('.delete-btn').forEach(button => {
            button.addEventListener('click', function() {
                if (confirm('Are you sure you want to delete this property?')) {
                    // In a real application, you would send a delete request to the server
                    alert('Property deleted successfully!');
                }
            });
        });
        
        // View property
        document.querySelectorAll('.view-btn').forEach(button => {
            button.addEventListener('click', function() {
                // Redirect to property detail page
                window.location.href = 'property-detail.html';
            });
        });
             // Form submission
        document.getElementById('contact-form').addEventListener('submit', function(e) {
            e.preventDefault();
            
            // In a real application, you would send the form data to a server here
            alert('Thank you for your message! We will get back to you soon.');
            
            // Reset form
            this.reset();
        });
        // Change main image when thumbnail is clicked
        function changeImage(src) {
            document.getElementById('main-image').src = src;
        }
        
        // Open modal with clicked image
        document.querySelectorAll('.image-gallery img').forEach(img => {
            img.addEventListener('click', function() {
                document.getElementById('modal-image').src = this.src;
                document.getElementById('image-modal').style.display = 'flex';
            });
        });
        
        // Close modal
        function closeModal() {
            document.getElementById('image-modal').style.display = 'none';
        }
        
        // Close modal when clicking outside the image
        window.onclick = function(event) {
            const modal = document.getElementById('image-modal');
            if (event.target === modal) {
                modal.style.display = 'none';
            }
        }