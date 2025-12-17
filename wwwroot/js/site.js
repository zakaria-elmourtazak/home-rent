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
//  document.getElementById("sortBySelectForm").addEventListener("change", function() {
//             // Submit the form when sort option changes
//             document.getElementById("filter-sort-form").submit();
//         });
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
            // e.preventDefault();
            
            // In a real application, you would send the form data to a server here
            
            // Reset form
            // this.reset();
            // document.getElementById('image-preview').innerHTML = '';
        });
        document.querySelectorAll('.delete-btn').forEach(btn => {
    btn.addEventListener('click', async () => {
        const id = btn.dataset.id;
        if (!id) return;
        if (!confirm('Are you sure you want to delete this property?')) return;

        // get antiforgery token from page
        const tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenEl ? tokenEl.value : '';

        try {
            const formData = new FormData();
            formData.append('id', id);
            // include the antiforgery token
            if (token) formData.append('__RequestVerificationToken', token);

            const res = await fetch('/Property/Delete', {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: formData
            });

            if (!res.ok) throw new Error('Delete failed');

            const data = await res.json().catch(() => null);
            // if JSON response, remove row from DOM; otherwise reload
            if (data && data.success) {
                // try remove the table row containing this button
                const row = btn.closest('tr');
                if (row) row.remove();
                else location.reload();
            } else {
                location.reload();
            }
        } catch (err) {
            console.error(err);
            alert('Failed to delete property.');
        }
    });
});
        const addPropertyBtn = document.getElementById('add-property-btn');
        console.log('Add Property Button:', addPropertyBtn);
        const propertyFormContainer = document.getElementById('property-form-container');
        const formTitle = document.getElementById('form-title');
        const propertyForm = document.getElementById('property-form');
        const cancelBtn = document.getElementById('cancel-btn');
        const imageUpload = document.getElementById('image-upload');
        const imagePreview = document.getElementById('image-preview');
        
        // Show add property form
        addPropertyBtn.addEventListener('click', () => {
            console.log('Add Property button clicked');
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
        // propertyForm.addEventListener('submit', function(e) {
        //     e.preventDefault();
            
        //     // Get form data
        //     const propertyId = document.getElementById('property-id').value;
        //     const title = document.getElementById('property-title').value;
        //     const description = document.getElementById('property-description').value;
        //     const type = document.getElementById('property-type').value;
        //     const price = document.getElementById('property-price').value;
        //     const bedrooms = document.getElementById('property-bedrooms').value;
        //     const bathrooms = document.getElementById('property-bathrooms').value;
        //     const sqft = document.getElementById('property-sqft').value;
        //     const date = document.getElementById('property-date').value;
        //     const street = document.getElementById('property-street').value;
        //     const city = document.getElementById('property-city').value;
        //     const state = document.getElementById('property-state').value;
        //     const zip = document.getElementById('property-zip').value;
            
        //     // In a real application, you would send this data to a server
        //     if (propertyId) {
        //         // Update existing property
        //         alert(`Property "${title}" updated successfully!`);
        //     } else {
        //         // Add new property
        //         alert(`Property "${title}" added successfully!`);
        //     }
            
        //     // Hide form
        //     propertyFormContainer.classList.add('hidden');
        // });
          try { loadProducts(); } catch (e) { console.error('loadProducts error', e); }
    try { loadCart(); } catch (e) { console.error('loadCart error', e); }

// ...existing code...
// updated renderPreviews with idempotent guard to avoid duplicate renders
function renderPreviews(files, target) {
    if (!target || !files || files.length === 0) return;

    // create a signature for the file list to avoid re-rendering the same files
    const sig = Array.from(files).map(f => `${f.name}:${f.size}:${f.lastModified}`).join('|');
    if (target.dataset.lastSig === sig) return; // already rendered same files
    target.dataset.lastSig = sig;

    target.innerHTML = '';

    for (let i = 0; i < files.length; i++) {
        const file = files[i];
        if (!file.type.match('image.*')) continue;

        const reader = new FileReader();
        reader.onload = function (e) {
            const previewItem = document.createElement('div');
            previewItem.className = 'image-preview-item inline-block mr-2 mb-2';

            const img = document.createElement('img');
            img.src = e.target.result;
            img.alt = 'Property Image';
            img.className = 'w-24 h-24 object-cover rounded';

            const removeBtn = document.createElement('span');
            removeBtn.className = 'remove-image cursor-pointer text-red-500 ml-1';
            removeBtn.innerHTML = '&times;';
            removeBtn.onclick = function () {
                previewItem.remove();
                // update signature after removal to allow re-render if needed
                target.dataset.lastSig = Array.from(target.querySelectorAll('img'))
                    .map(img => img.src).join('|');
            };

            previewItem.appendChild(img);
            previewItem.appendChild(removeBtn);
            target.appendChild(previewItem);
        };
        reader.readAsDataURL(file);
    }
}

// attach handler only once
(function () {
    let imageUploadEl = document.getElementById('image-upload');
    const imagePreviewEl = document.getElementById('image-preview');
    if (!imageUploadEl || !imagePreviewEl) return;

    // Replace the element with a clone to remove any previously attached event listeners
    // (defensive: solves duplicate listeners if script accidentally ran more than once)
    const newInput = imageUploadEl.cloneNode(true);
    imageUploadEl.parentNode.replaceChild(newInput, imageUploadEl);
    imageUploadEl = newInput;

    // Debounced single handler
    function onImageChange(e) {
        // small debounce to collapse any rapid multiple events
        if (imageUploadEl._previewTimeout) clearTimeout(imageUploadEl._previewTimeout);
        imageUploadEl._previewTimeout = setTimeout(() => {
            renderPreviews(e.target.files, imagePreviewEl);
            imageUploadEl._previewTimeout = null;
        }, 10);
    }

    // Attach only once
    if (!imageUploadEl.dataset.previewHandlerAttached) {
        imageUploadEl.addEventListener('change', onImageChange);
        imageUploadEl.dataset.previewHandlerAttached = '1';
    }
})();


document.addEventListener('click', function (e) {
  document.querySelectorAll('.dropdown').forEach(dropdown => {
    if (dropdown.contains(e.target)) {
      dropdown.classList.add('open');
    } else {
      dropdown.classList.remove('open');
    }
  });
});

document.addEventListener('keydown', function (e) {
  if (e.key === 'Escape') {
    document.querySelectorAll('.dropdown.open').forEach(d => d.classList.remove('open'));
  }
});
// ...existing code...
    // helper to render previews
    // function renderPreviews(files, target) {
    //     if (!target) return;
    //     target.innerHTML = '';
    //     for (let i = 0; i < files.length; i++) {
    //         const file = files[i];
    //         if (!file.type.match('image.*')) continue;
    //         const reader = new FileReader();
    //         reader.onload = function (e) {
    //             const previewItem = document.createElement('div');
    //             previewItem.className = 'image-preview-item inline-block mr-2 mb-2';
    //             const img = document.createElement('img');
    //             img.src = e.target.result;
    //             img.alt = 'Property Image';
    //             img.className = 'w-24 h-24 object-cover rounded';
    //             const removeBtn = document.createElement('span');
    //             removeBtn.className = 'remove-image cursor-pointer text-red-500 ml-1';
    //             removeBtn.innerHTML = '&times;';
    //             removeBtn.onclick = function () { previewItem.remove(); };
    //             previewItem.appendChild(img);
    //             previewItem.appendChild(removeBtn);
    //             target.appendChild(previewItem);
    //         };
    //         reader.readAsDataURL(file);
    //     }
    // }

    // if (imageUpload && imagePreview) {
    //     imageUpload.addEventListener('change', function (e) {
    //         renderPreviews(e.target.files, imagePreview);
    //     });
    // }

    if (propertyForm) {
        // If you want default form submit to post to server, do nothing here.
        // If you want AJAX, uncomment e.preventDefault() and implement fetch.
        propertyForm.addEventListener('submit', function (e) {
            // e.preventDefault();
            // keep default submit unless you implement ajax
            // if you see an unexpected alert or redirect, check for other scripts calling alert/redirect
        });
    }

    if (addPropertyBtn && propertyFormContainer) {
        addPropertyBtn.addEventListener('click', () => {
            if (formTitle) formTitle.textContent = 'Add New Property';
            propertyFormContainer.classList.remove('hidden');
            if (propertyForm) propertyForm.reset();
            if (imagePreview) imagePreview.innerHTML = '';
            const idInput = document.getElementById('property-id');
            if (idInput) idInput.value = '';
        });
    }

    if (cancelBtn && propertyFormContainer) {
        cancelBtn.addEventListener('click', () => propertyFormContainer.classList.add('hidden'));
    }
         // Attach edit / delete / view handlers safely (elements may be added dynamically)
    document.querySelectorAll('.edit-btn').forEach(button => {
        button.addEventListener('click', async function () {
            if (!button.dataset.id) return;
            const id = button.dataset.id;
            formTitle && (formTitle.textContent = 'Edit Property');
            propertyFormContainer && propertyFormContainer.classList.remove('hidden');

            // Fetch property data from server if endpoint exists
            try {
                const res = await fetch(`/Property/Get/${id}`);
                if (!res.ok) throw new Error('Failed to fetch property');
                const p = await res.json();
                console.log('Fetched property:', p);
                // populate fields if they exist
                // const setVal = (sel, val) => { const el = document.getElementById(sel); if (el) el.value = val ?? ''; };
                  document.getElementById('property-id').value =  p.id;
                  document.getElementById('property-title').value =  p.title
                  document.getElementById('property-description').value =  p.description
                  document.getElementById('property-type').value =  p.propertyType
                  document.getElementById('property-price').value =  p.pricePerMonth
                  document.getElementById('property-bedrooms').value =  p.bedrooms
                  document.getElementById('property-bathrooms').value =  p.bathrooms
                  document.getElementById('property-sqft').value =  p.squareFeet
                  document.getElementById('property-date').value =  p.availableDate
                  document.getElementById('property-street').value =  p.streetAddress
                  document.getElementById('property-city').value =  p.city
                  document.getElementById('property-state').value =  p.state
                  document.getElementById('property-zip').value =  +p.zipCode
                  document.getElementById('property-country').value =  p.country
                // show existing images
                if (imagePreview) {
                    imagePreview.innerHTML = '';
                    if (p.Images && p.Images.length) {
                        p.Images.forEach(src => {
                            const img = document.createElement('img');
                            img.src = src;
                            img.className = 'inline-block w-24 h-24 object-cover mr-2 mb-2 rounded';
                            imagePreview.appendChild(img);
                        });
                    }
                }
                // switch form action to edit (server expects /Property/Edit)
                if (propertyForm) propertyForm.action = '/Property/Edit';
            } catch (err) {
                console.error(err);
                alert('Error loading property data');
            }
        });
    });

    // document.querySelectorAll('.delete-btn').forEach(button => {
    //     button.addEventListener('click', function () {
    //         if (!button.dataset.id) return;
    //         if (!confirm('Are you sure you want to delete this property?')) return;
    //         // implement server delete call if desired, e.g. fetch('/Property/Delete?id=ID', { method: 'POST', ...})
    //         alert('Property deleted successfully! (stub)');
    //     });
    // });

    document.querySelectorAll('.view-btn').forEach(button => {
        button.addEventListener('click', function () {
            if (!button.dataset.id) return;
            window.location.href = `/Home/PropertyDetail?id=${button.dataset.id}`;
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

        document.querySelector(".profile-trigger").addEventListener("click", () => {
    document.querySelector(".profile-dropdown").classList.toggle("open");
});

document.addEventListener("click", function (e) {
    if (!document.querySelector(".profile-menu").contains(e.target)) {
        document.querySelector(".profile-dropdown").style.display = "none";
    }
});

function togglePassword(id, btn) {
    const input = document.getElementById(id);
    if (!input) return;
    const icon = btn && btn.querySelector('i');
    if (input.type === 'password') {
        input.type = 'text';
        if (icon) { icon.classList.remove('fa-eye'); icon.classList.add('fa-eye-slash'); }
        btn && btn.setAttribute('aria-pressed', 'true');
    } else {
        input.type = 'password';
        if (icon) { icon.classList.remove('fa-eye-slash'); icon.classList.add('fa-eye'); }
        btn && btn.setAttribute('aria-pressed', 'false');
    }
}

